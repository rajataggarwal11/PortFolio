using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace USER_DETAIL
{
    [Serializable]
    public class USER_MASTER
    {
        
        #region Properties
        public int USER_ID { get; set; }
        public string PASSWORD { get; set; }
        public string CONFIRM_PASSWORD { get; set; }
        public string EMAIL_ID { get; set; }
        public string MOBILE_NUMBER { get; set; }
        public string ACTIVE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string MODIFIED_ON { get; set; }
        #endregion

        #region Insert/Update Operation
        public string SaveData()
        {
            string Msg = "";
            if (string.IsNullOrEmpty(this.EMAIL_ID))
            {
                Msg += "Please enter mail id";
            }
            if (string.IsNullOrEmpty(this.MOBILE_NUMBER))
            {
                Msg += "Please enter mobile number";
            }
            if (string.IsNullOrEmpty(this.PASSWORD))
            {
                Msg = "Please ennter password";
            }

            Hashtable ht = new Hashtable();
            ht.Add("EMAIL_ID", this.EMAIL_ID);
            ht.Add("MOBILE_NUMBER", this.MOBILE_NUMBER);
            ht.Add("PASSWORD", this.PASSWORD);
            if (Msg.Length == 0)
            {
                Msg = DataAccessLayer.DataAccess.ExecuteDMLQuery("SIGNUPUSER", CommandType.StoredProcedure, ht);
            }
           
            return Msg;
        }
        #endregion

    }
    [Serializable]

    public class USER_MASTERs :CollectionBase
    {
        public void LogInUser(string Email_id,string Mobile_number,string Password)
        {

            Hashtable ht = new Hashtable();
            ht.Add("EMAIL_ID", Email_id);
            ht.Add("MOBILE_NUMBER", Mobile_number);
            ht.Add("PASSWORD", Password);

            DataTable dt = DataAccessLayer.DataAccess.GetDataTable("LOGINUSER", CommandType.StoredProcedure, ht);

            USER_MASTER objUSER_MASTER = new USER_MASTER();
            FillProperties(DataAccessLayer.DataAccess.GetObjectArraylist(objUSER_MASTER.GetType(), dt));
        }
        public void FillProperties(ArrayList objects)
        {
            this.Clear();
            foreach (USER_MASTER objUSER_MASTER in objects)
            {
                this.Add(objUSER_MASTER);
            }
        }

        public int Add(USER_MASTER value)
        {
            int newIndex =List.Add(value);
            return (newIndex);
        }

        public USER_MASTER this[int index]
        {
            get
            {
                return ((USER_MASTER)this.List[index]);
            }
            set
            {
                this.List[index] = value;
            }
        }

    }
}
