using AppointmentScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace AppointmentScheduler.Controllers
{
    public class UsersController : Controller
    {
        IConfiguration _configuration;
        SqlConnection _Connection;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
            _Connection = new SqlConnection(_configuration.GetConnectionString("Appointment"));
        }

        public List<UsersModel> GetAppointment(int login)
        {
            List<UsersModel> allAppointment = new();
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("Fetch_Appointment", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", login);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                UsersModel user = new();
                Console.WriteLine("Inside the while block");
                Console.WriteLine(dr.GetString(2));
                user.AppointId = (int)dr[0];
                user.Id = (int)dr[1];
                user.UserName = (string)dr[2];
                user.Email = (string)(dr[3]);
                user.ContactNo = Convert.ToInt64(dr[4]);
                user.Password = dr.GetString(5);
                user.AppointmentDate = Convert.ToDateTime(dr[6]);
                user.AppointmentDesc = (string)dr[7];
                allAppointment.Add(user);
            }
            dr.Close();
            _Connection.Close();
            return allAppointment;
        }
        // GET: UsersController
        public ActionResult Index(int log)
        {
            return View(GetAppointment(log));
        }
        
        // GET: UsersController/Details/5
        public ActionResult Details(int AppointId)
        {
            return View(GetAppointmentbyId(AppointId));
        }

        UsersModel GetAppointmentbyId(int id)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("GetAppointmentbyId", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointId", id);
            SqlDataReader dr = cmd.ExecuteReader();
            UsersModel user = new();
            while (dr.Read())
            {
                user.AppointId = (int)dr[0];
                user.Id = (int)dr[1];
                user.UserName = (string)dr[2];
                user.Email = (string)(dr[3]);
                user.ContactNo = Convert.ToInt64(dr[4]);
                user.Password = dr.GetString(5);
                user.AppointmentDate = Convert.ToDateTime(dr[6]);
                user.AppointmentDesc = (string)dr[7];
            }
            dr.Close ();
            _Connection.Close();
            return user;
        }
        // GET: UsersController/Create
        public ActionResult Create()
        {
                return View();
        }

        void InsertAppointment(UsersModel user)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("InsertAppointment", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@App_Date", user.AppointmentDate);
            cmd.Parameters.AddWithValue("@App_Desc", user.AppointmentDesc);
            cmd.ExecuteNonQuery();
            _Connection.Close();
        }
        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsersModel user)
        {
            try
            {
                InsertAppointment(user);
                return RedirectToAction("Index", "Users", new { log = user.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int AppointId)
        {
            return View(GetAppointmentbyId(AppointId));
        }
        void UpdateAppointment(int appointId, UsersModel user)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("UpdateAppoint", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointId", appointId);
            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@ContactNo", user.ContactNo);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@App_Date", user.AppointmentDate);
            cmd.Parameters.AddWithValue("@App_Desc", user.AppointmentDesc);
            cmd.ExecuteNonQuery();
            _Connection.Close();
        }
        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsersModel user)
        {
            try
            {
                UpdateAppointment(id, user);
                return RedirectToAction("Index", "Users", new { log = user.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(GetAppointmentbyId(id));
        }
        void DeleteAppoint(int AppointId)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("DeleteAppointment", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AppointId", AppointId);
            cmd.ExecuteNonQuery();
            _Connection.Close();
        }
        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int AppointId,UsersModel users)
        {
            try
            {
                DeleteAppoint(AppointId);
                return RedirectToAction("Index", "Users", new { log = users.Id });
            }
            catch
            {
                return View();
            }
        }
    }
}
