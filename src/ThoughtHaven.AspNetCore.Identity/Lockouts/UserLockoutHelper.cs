﻿using System;
using System.Threading.Tasks;
using ThoughtHaven.AspNetCore.Identity.Lockouts;
using ThoughtHaven.Data;

namespace ThoughtHaven.AspNetCore.Identity
{
    public abstract partial class UserHelper
    {
        protected abstract ICrudStore<string, TimedLockout> TimedLockoutStore { get; }

        public virtual UserMessage LockedOut { get; }
            = new UserMessage("This account has been locked to protect it from possible hacking. Wait a few minutes to try again.");

        public virtual Task<bool> IsLockedOut(string key) =>
            this.IsLockedOut(key, TimeSpan.FromMinutes(10), maxFailedAccessAttempts: 5);

        protected virtual async Task<bool> IsLockedOut(string key, TimeSpan lockoutLength,
            byte maxFailedAccessAttempts)
        {
            Guard.NullOrWhiteSpace(nameof(key), key);
            Guard.LessThan(nameof(maxFailedAccessAttempts), maxFailedAccessAttempts,
                minimum: 2);

            var now = this.Clock.UtcNow;

            var lockout = await this.TimedLockoutStore.Retrieve(key).ConfigureAwait(false);

            if (lockout == null)
            {
                lockout = new TimedLockout(key, lastModified: now);

                await this.TimedLockoutStore.Create(lockout).ConfigureAwait(false);
            }
            else if (!lockout.Expiration.HasValue)
            {
                if (lockout.LastModified <= now.Subtract(lockoutLength))
                {
                    lockout = new TimedLockout(key, lastModified: now);

                    await this.TimedLockoutStore.Update(lockout).ConfigureAwait(false);
                }
                else
                {
                    lockout.LastModified = now;
                    lockout.FailedAccessAttempts++;

                    if (lockout.FailedAccessAttempts >= maxFailedAccessAttempts)
                    {
                        lockout.Expiration = now.Add(lockoutLength);
                    }

                    await this.TimedLockoutStore.Update(lockout).ConfigureAwait(false);
                }
            }
            else if (lockout.Expiration <= now)
            {
                lockout = new TimedLockout(key, lastModified: now);

                await this.TimedLockoutStore.Update(lockout).ConfigureAwait(false);
            }

            return lockout.Expiration.HasValue && lockout.Expiration > now;
        }
    }
}