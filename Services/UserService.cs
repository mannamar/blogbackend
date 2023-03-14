using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogbackend.Models;
using blogbackend.Models.DTO;
using blogbackend.Services.Context;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace blogbackend.Services
{
    public class UserService : ControllerBase
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public bool DoesUserExist(string? Username)
        {
            // Check the table to see if the username exists
            // If 1 item matches the condition, return the item
            // If no item matches the condition, return null
            // If multiple items matche, an error will occur

            return _context.UserInfo.SingleOrDefault( user => user.Username == Username ) != null;
        }
        public bool AddUser(CreateAccountDTO UserToAdd)
        {
            // If user already exists
            // If they do not exist, create account
            // Else throw a false
            bool result = false;

            // If the user does not exist
            if(!DoesUserExist(UserToAdd.Username))
            {
                // Create a new instance of user model (empty object)
                UserModel newUser = new UserModel();
                // Create a salt and a hash password
                var hashPassword = HashPassword(UserToAdd.Password);
                newUser.Id = UserToAdd.Id;
                newUser.Username = UserToAdd.Username;
                newUser.Salt = hashPassword.Salt;
                newUser.Hash = hashPassword.Hash;

                // Adding newUser to our database
                _context.Add(newUser);
                // This saves to our database and return the number of entries that was written to the database

                // _context.SaveChanges();
                result = _context.SaveChanges() != 0;
            }

            return result;
            // else throw a false
        }

        public PasswordDTO HashPassword(string? password)
        {
            PasswordDTO newHashedPassword = new PasswordDTO();

            // This is a byte array with a length of 64
            byte[] SaltByte = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            // Enhanced RNG of numbers without using zero
            provider.GetNonZeroBytes(SaltByte);
            // Encoding the 64 digits to string
            // Salt makes the hash unique to the user
            // If we only had a hash password, if people have the same password the hash would be the same
            var Salt = Convert.ToBase64String(SaltByte);

            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltByte, 10000);

            // Encoding our password with out salt
            // Brute force would take decades
            var Hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            newHashedPassword.Salt = Salt;
            newHashedPassword.Hash = Hash;

            return newHashedPassword;
        }

        public bool VerifyUserPassword(string? Password, string? storedHash, string? storedSalt)
        {
            // Get our existing salt and change it to a base 64 string
            var SaltBytes = Convert.FromBase64String(storedSalt);
            // Making the password that the user inputed and using the stored salt
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Password, SaltBytes, 10000);
            // Created the new hash
            var newHash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            // Checking if the new hash is the same as the stored hash
            return newHash == storedHash;
        }

        public IActionResult Login(LoginDTO User)
        {
            // Return an error code if the user does not have a valid username or password
            IActionResult Result = Unauthorized();

            // Check to see if the user exists
            if (DoesUserExist(User.Username))
            {
                // True
                // Store user object
                // Create another helper function
                UserModel foundUser = GetUserByUsername(User.Username);
                // Check if the password is correct
                if (VerifyUserPassword(User.Password, foundUser.Hash, foundUser.Salt))
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:5000",
                        audience: "http://localhost:5000",
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    Result = Ok(new { Token = tokenString });
                }
            }

            return Result;
        }

        public UserModel GetUserByUsername(string? username)
        {
            return _context.UserInfo.SingleOrDefault(user => user.Username == username);
        }
    }
}