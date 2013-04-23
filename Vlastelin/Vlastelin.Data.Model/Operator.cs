using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common.Attributes;
using Vlastelin.Common;
using System.ComponentModel;

namespace Vlastelin.Data.Model
{
    public enum Roles
    {
        [DescriptionAttribute("Администратор")]Administrator = 0,
		[DescriptionAttribute("Кассир")]User = 1
    }

    public sealed class Operator
        : FIOItem
    {
        [FieldName("BranchId")]
        public long branchId { get; set; }
        private Branch _branch;
        public Branch Branch
        {
            get { return _branch; }
            set
            {
				Ct.CheckRequireField("Филиал", value);

                branchId = value.Id;
                _branch = value;
            }
        }

        public Roles Role { get; set; }

        public Operator()
            :base()
        {
        }

        public Operator(int id, String name, String surname, String patr, Roles role)
            :this()
        {
            this.Id = id;
            this.Name = name;
            this.Surname = surname;
            this.Patronymic = patr;
            this.Role = role;
        }

        [FieldName("UserId")]
        public string _userId { get; set; }
        private VlastelinMembershipUser _mUser;
        public VlastelinMembershipUser User
        {
            get { return _mUser; }
            set
            {
                _userId = value != null ? value.id : String.Empty;
                _mUser = value;
            }
        }

        public string Login { get { return User.Username; } }
        public DateTime LastPasswordChangeDate { get { return User.LastPasswordChangedDate; } }
        public DateTime LastActivityDate { get { return User.LastActivityDate; } }
        public DateTime LastLoginDate { get { return User.LastLoginDate; } }
        public DateTime CreationDate { get { return User.CreationDate; } }
        public int FailedPasswordAttemptCount { get { return User.FailedPasswordAttemptCount; } }

        public override bool Equals(object obj)
        {
            if (!(obj is Operator)) return false;
            Operator op = obj as Operator;
            return base.Equals(obj) && op.branchId.Equals(this.branchId);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ branchId.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
