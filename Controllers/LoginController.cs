using AppointmentScheduler.Models;
using Azure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics.Metrics;
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
            cmd.Parameters.AddWithValue("@User_id", login_cred.Id);
            cmd.Parameters.AddWithValue("@Email",login_cred.Email);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Console.WriteLine(dr["Password"]);
                string psw = (string)dr["Password"];
                if(psw == login_cred.Password)
                {
                    return true;
                }
            }
            _Connection.Close();

            return false;
        }

        int RetriveId (LoginModel log)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Fetch_detail", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email",log.Email);
            cmd.Parameters.AddWithValue("@UserName", log.UserName);
            SqlDataReader dr = cmd.ExecuteReader();
            int id=0;
            while (dr.Read())
            {
                id = (int)dr[0];
            }
            dr.Close();
            _Connection.Close();
            return id;
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
                else
                {
                    int log_user = RetriveId(login);
                    return RedirectToAction("Index", "Users", new { log = log_user });
                }/*
                else
                {
                    return RedirectToAction("Login", "Login");
                }*/

            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View(GetUsers(id));
        }
        LoginModel GetUsers(int id)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Get_User", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            SqlDataReader reader = cmd.ExecuteReader();
            LoginModel log = new();
            while (reader.Read())
            {
                
                log.Id = (int)reader[0];
                log.UserName =(string) reader[1];
                log.Password =(string) reader[4];
                log.ContactNo = (Int64)reader[3];
                log.Email = (string)reader[2];
                log.Cmpy_name= (string)reader[5];
            }
            reader.Close();
            _Connection.Close();
            return log;
        }
        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(GetUsers(id));
        }
        void EditUser(int id, LoginModel log)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Edit_User", _Connection);
            cmd.CommandType=CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("UserName", log.UserName);
            cmd.Parameters.AddWithValue("Email", log.Email);
            cmd.Parameters.AddWithValue("ContactNo", log.ContactNo);
            cmd.Parameters.AddWithValue("Password", log.Password);
            cmd.Parameters.AddWithValue("Cmpy_name", log.Cmpy_name);
            cmd.ExecuteNonQuery();
            _Connection.Close();
        }
        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, LoginModel log)
        {
            try
            {
                EditUser(id,log);
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
        void DeleteUser(int id)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Delete_user", _Connection);
            cmd.CommandType= CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
            _Connection.Close();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LoginModel log)
        {
            try
            {
                DeleteUser(id) ;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
