using AppointmentScheduler.Models;
using Azure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Numerics;
using System.Security.Principal;

namespace AppointmentScheduler.Controllers
{
    public class LoginController : Controller
    {
        IConfiguration _configuration;
        SqlConnection _Connection;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _Connection = new SqlConnection(_configuration.GetConnectionString("LogIn"));
        }

        public List<LoginModel> GetUsers()
        {
            List<LoginModel> allUsers = new();
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Fetch_Users", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                LoginModel login_user = new();
                login_user.Id = (int)dr[0];
                login_user.UserName = (string)dr[1];
                login_user.Email= (string) (dr[2]);
                login_user.ContactNo = Convert.ToInt64(dr[3]);
                login_user.Cmpy_name = dr.GetString(5);
                login_user.Password = dr.GetString(4);
                allUsers.Add(login_user);
            }
            dr.Close();
            _Connection.Close();
            return allUsers;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View(GetUsers());
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        //For SignUp Page
        // GET: AdminController/Create
        public ActionResult SignUp()
        {
            return View();
        }

        void CreateUser(LoginModel login_user)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Insert_User", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_name", login_user.UserName);
            cmd.Parameters.AddWithValue("@Email", login_user.Email);
            cmd.Parameters.AddWithValue("@ContactNo", login_user.ContactNo);
            cmd.Parameters.AddWithValue("@Password", login_user.Password);
            cmd.Parameters.AddWithValue("@Company_Name", login_user.Cmpy_name);

            cmd.ExecuteNonQuery();
            _Connection.Close();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(LoginModel login)
        {
            try
            {
                CreateUser(login);
                return RedirectToAction(nameof(Login));
            }
            catch(Exception e)
            {
                Console.WriteLine($"We have faced some issues {e}");
                return View();
            }
        }

        //Login Page
        // GET: AdminController/Create
        public ActionResult Login()
        {
            return View();
        }
        bool LoginValidation(LoginModel login_cred)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Retrive_Password", _Connection);
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("UserId", login_cred.Id);
            cmd.Parameters.AddWithValue("@Email",login_cred.Email);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Console.WriteLine(dr[0]);
                string psw = (string)dr[0];
                if(psw == login_cred.Password)
                {
                    return true;
                }
            }
            _Connection.Close();

            return false;
        }
        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel login)
        {
            try
            {
                if(login.UserName=="admin12345" && login.Password == "psw")
                {
                    return RedirectToAction("Index", "Login");
                }
                if (LoginValidation(login))
                {
                    Console.WriteLine(LoginValidation(login));
                    return RedirectToAction("Index","User");
                }
                else
                {
                    Console.WriteLine(LoginValidation(login));
                    var response = new
                    {
                        message = "Your alert message here"
                    };
                    Response.WriteAsJsonAsync(response);
                    return RedirectToAction("Login", "Login");
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
