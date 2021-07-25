using AutoMapper;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Sam.Common.Util;
using SAM.Web.Models;
using SAM.Web.Context;

namespace SAM.Web.Common
{
    public static class UserCommon
    {
        public static User CurrentUser()
        {
            User usuario = new User();

            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    usuario = (User)HttpContext.Current.Session["UsuarioAutenticado"];
 
                }

                return usuario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public static RelationshipUserProfile CurrentRelationshipUsersProfile(int userId)
        //{
        //    //Provável correção para erros iguais ao de Id 46320
        //    if (db.Database.Connection.State == System.Data.ConnectionState.Open) {
        //        db.Database.Connection.Close();
        //    }

        //    db.Configuration.AutoDetectChangesEnabled = true;
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //        return db.RelationshipUserProfiles.Where(m => m.UserId == userId && m.DefaultProfile == true).FirstOrDefault();
        //    return null;
        //}

        public static string EncryptPassword(string _Password)
        {
            StringBuilder encryptedPassword = new StringBuilder();
            string _return = System.String.Empty;
            MD5 md5 = MD5.Create();
            byte[] input = Encoding.ASCII.GetBytes(_Password);
            byte[] hash = md5.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                encryptedPassword.Append(hash[i].ToString("X2"));
            }
            _return = encryptedPassword.ToString();
            return _return;
        }

        //public static RelationshipUserProfileInstitution CurrentProfileLogin(int rupId)
        //{
        //    //Provável correção para erros iguais ao de Id 46999
        //    if (db.Database.Connection.State == System.Data.ConnectionState.Open) {
        //        db.Database.Connection.Close();
        //    }

        //    db.Configuration.AutoDetectChangesEnabled = true;

        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //        return db.RelationshipUserProfileInstitutions.FirstOrDefault(p => p.RelationshipUserProfileId == rupId && p.Status == true);
        //    return null;
        //}

        //public static AuthenticatedUser CurrentUser()
        //{
        //    if (HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        User user = db.Users.FirstOrDefault(m => m.CPF == HttpContext.Current.User.Identity.Name);

        //        Mapper.CreateMap<User, AuthenticatedUser>();
        //        Mapper.Map<AuthenticatedUser>(user);
        //    }
        //    return null;
        //}
    }

    public class ComumLayout {

        private UsuarioContext db = new UsuarioContext();

        public User CurrentUser()
        {
            User usuario = new User();

            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    usuario = (User)HttpContext.Current.Session["UsuarioAutenticado"];

                }

                return usuario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RelationshipUserProfile CurrentRelationshipUsersProfile(int userId)
        {
            db.Configuration.AutoDetectChangesEnabled = true;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfiles.Where(m => m.UserId == userId && m.DefaultProfile == true).FirstOrDefault();
            return null;
        }

        public SAM.Web.Models.Profile CurrentProfile(int relationshipUserProfileId)
        {
            db.Configuration.AutoDetectChangesEnabled = true;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfiles.FirstOrDefault(m => m.DefaultProfile == true && m.Id == relationshipUserProfileId).RelatedProfile;
            return null;
        }

        public RelationshipUserProfileInstitution CurrentProfileLogin(int profileId)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return db.RelationshipUserProfileInstitutions.FirstOrDefault(p => p.RelatedRelationshipUserProfile.Id == profileId && p.Status == true);
            return null;
        }

        public Institution GetInstitution(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return db.Institutions.FirstOrDefault(p => p.Id == Id);
            return null;
        }

        public string GetInstitutionCabecalho(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                return (from i in db.Institutions where i.Id == Id select i.Code.ToString() + " - " + i.Description).FirstOrDefault();
            return string.Empty;
        }

        public string GetBudgetUnit(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var retorno = db.BudgetUnits.FirstOrDefault(p => p.Id == Id);
                if (retorno != null)
                    return retorno.Code.ToString();
                else
                    return string.Empty;
            }
            return string.Empty;
        }

        public ManagerUnit GetManagerUnit(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var retorno = db.ManagerUnits.FirstOrDefault(p => p.Id == Id);
                if (retorno != null)
                    return retorno;
                else
                    return new ManagerUnit();
            }
            return null;
        }
        
        public string GetAdministrativeUnit(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var retorno = db.AdministrativeUnits.FirstOrDefault(p => p.Id == Id);
                if (retorno != null)
                    return retorno.Code.ToString();
                else
                    return string.Empty;
            }
            return string.Empty;
        }

        public Section GetSection(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var retorno = db.Sections.FirstOrDefault(p => p.Id == Id);
                if (retorno != null)
                    return retorno;
                else
                    return new Section();

            }
            return null;
        }

        public string GetSectionCabecalho(int Id)
        {
            db.Configuration.AutoDetectChangesEnabled = true;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var retorno = db.Sections.FirstOrDefault(p => p.Id == Id);
                if (retorno != null)
                    return retorno.Code.ToString() + " - " + retorno.Description;
                else
                    return string.Empty;

            }
            return string.Empty;
        }

    }
}