using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;

namespace Vlastelin.Data.Model
{
    public sealed class VlastelinMembershipUser
    {
        [FieldName("appName")]
        public string ApplicationName { get; set; }

        [FieldName("id")]
        public string id { get; set; }

        public Guid userId { get { return Guid.Parse(id); } set { id = value.ToString(); } }

        [FieldName("Username")]
        public string Username { get; set; }

        [FieldName("Password")]
        public string Password { get; set; }

        [FieldName("LastPasswordChangedDate")]
        public DateTime LastPasswordChangedDate { get; set; }

        [FieldName("PasswordQuestion")]
        public string PasswordQuestion { get; set; }

        [FieldName("PasswordAnswer")]
        public string PasswordAnswer { get; set; }

        //[FieldName("isOnline")]
        //public bool isOnline { get { return LastActivityDate } set; }

        [FieldName("LastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        [FieldName("Email")]
        public string Email { get; set; }

        [FieldName("isApproved")]
        public bool isApproved { get; set; }

        [FieldName("isLocked")]
        public bool isLocked { get; set; }

        [FieldName("Comment")]
        public string Comment { get; set; }

        [FieldName("FailedPasswordAttemptCount")]
        public int FailedPasswordAttemptCount { get; set; }

        [FieldName("FailedPasswordAttemptWindowStart")]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }

        [FieldName("FailedPasswordAnswerAttemptCount")]
        public int FailedPasswordAnswerAttemptCount { get; set; }

        [FieldName("FailedPasswordAnswerAttemptWindowStart")]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        [FieldName("CreationDate")]
        public DateTime CreationDate { get; set; }

        [FieldName("LastActivityDate")]
        public DateTime LastActivityDate { get; set; }

        [FieldName("LastLockedOutDate")]
        public DateTime LastLockedOutDate { get; set; }

        [FieldName("LastLockOutDate")]
        public DateTime LastLockOutDate { get; set; }
    }
}
