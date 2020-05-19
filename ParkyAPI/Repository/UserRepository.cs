using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ParkyAPIdbContext _dbContext;
        private readonly AppSettings _appSettings;


        public UserRepository(ParkyAPIdbContext dbContext, 
            IOptions<AppSettings> appSettings)
        {
            this._dbContext = dbContext;

            // retreive the key register for the secret attribute in appSettings.json file
            this._appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = this._dbContext.Users.SingleOrDefault( me =>
                        me.Username == username && me.Password == password);

            if (user == null)
            {
                return null;
            }

            //if user is found, we generate a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // hide the user password on post
            user.Password = "";

            return user;
        }

        public bool IsUniqueUser(string username)
        {
            var user = this._dbContext.Users.SingleOrDefault(us =>
                      us.Username == username);

            return (user == null)? true : false;
        }

        public User Register(string username, string password)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Role="Admin"
            };

            this._dbContext.Users.Add(user);
            this._dbContext.SaveChanges();
            user.Password = "";

            return user;
        }
    }
}
