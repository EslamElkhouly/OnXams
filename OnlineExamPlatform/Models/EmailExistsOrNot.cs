
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OnlineExamPlatform.ViewModels;


namespace OnlineExamPlatform.Models
{

    public class EmailExistsOrNotForStudent : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userInfo = (NewStudentViewModel)validationContext.ObjectInstance;

            if (userInfo == null)
            {
                return new ValidationResult("Email is required");
            }

            using (var context = new OnXamsEntities())
            {
                var userEmail =
                    context.UserAuthentications.FirstOrDefault(
                        a => a.Email.ToLower().Equals(value.ToString().ToLower()));
                if (userEmail == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Mail already exists");
                }
            }


        }

    }
    public class EmailExistsOrNotForExaminer : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userInfo = (NewExaminerViewModel)validationContext.ObjectInstance;

            if (userInfo == null)
            {
                return new ValidationResult("Email is required");
            }

            using (var context = new OnXamsEntities())
            {
                var userEmail =
                    context.UserAuthentications.FirstOrDefault(
                        a => a.Email.ToLower().Equals(value.ToString().ToLower()));
                if (userEmail == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Mail already exists");
                }
            }

            //var userInfo = (NewProctorViewModel)validationContext.ObjectInstance;

            //if (userInfo.EmailAddress == null)
            //{
            //    return new ValidationResult("Email is required");
            //}

            //using (var context = new OnXamsEntities())
            //{
            //    var userEmail =
            //        context.UserAuthentications.FirstOrDefault(
            //            a => a.Email.ToLower().Equals(value.ToString().ToLower()));
            //    if (userEmail == null)
            //    {
            //        return ValidationResult.Success;
            //    }
            //    else
            //    {
            //        return new ValidationResult("Mail already exists");
            //    }
            //}

        }

    }
    public class EmailExistsOrNotForProctor : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userInfo = (NewProctorViewModel)validationContext.ObjectInstance;

            if (userInfo == null)
            {
                return new ValidationResult("Email is required");
            }

            using (var context = new OnXamsEntities())
            {
                var userEmail =
                    context.UserAuthentications.FirstOrDefault(
                        a => a.Email.ToLower().Equals(value.ToString().ToLower()));
                if (userEmail == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Mail already exists");
                }
            }

           

        }

    }
    public class EmailExistsOrNotForAdmin: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var userInfo = (NewAdminViewModel)validationContext.ObjectInstance;

            if (userInfo == null)
            {
                return new ValidationResult("Email is required");
            }

            using (var context = new OnXamsEntities())
            {
                var userEmail =
                    context.UserAuthentications.FirstOrDefault(
                        a => a.Email.ToLower().Equals(value.ToString().ToLower()));
                if (userEmail == null)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Mail already exists");
                }
            }

            //var userInfo = (NewProctorViewModel)validationContext.ObjectInstance;

            //if (userInfo.EmailAddress == null)
            //{
            //    return new ValidationResult("Email is required");
            //}

            //using (var context = new OnXamsEntities())
            //{
            //    var userEmail =
            //        context.UserAuthentications.FirstOrDefault(
            //            a => a.Email.ToLower().Equals(value.ToString().ToLower()));
            //    if (userEmail == null)
            //    {
            //        return ValidationResult.Success;
            //    }
            //    else
            //    {
            //        return new ValidationResult("Mail already exists");
            //    }
            //}

        }

    }


}