using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using BCare.Models;
using System.Security.Cryptography;
using System.Text;

namespace BCare.data
{
    public class BcareContext
    {
        public string ConnectionString { get; set; }
        public BcareContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM pharmaceutical_company", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User()
                        {
                            UserID = reader.GetInt32("Pharm_ID"),
                            FirstName = reader.GetString("Pharm_Name")
                        });
                    }
                }
            }

            return users;
        }
        public bool Login(string username , string password)
        {
            User user = new User();
            using (MySqlConnection conn = GetConnection())
            {
  
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM premission_for_users", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(username.Equals(reader.GetString("User_Name")))
                        {
                            var sha512 = SHA512.Create();
                            byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                            string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");
                            if (hashedPassword.Equals(reader.GetString("PW_Hash"))) {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void Register(int User_ID, string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string username, string password, bool isDoctor)
        {
            int permissionID=1;
            //int userId = GenerateAutoID();
            // User newUser = new User()
            //{
            //    UserID = userId,
            //    HMOID = HMOID,
            //    FirstName = firstname,
            //    LastName = lastname,
            //    Gender = new Gender (),
            //    BirthDate = Convert.ToDateTime(birhdate),
            //    BloodType = new BloodType(),
            //    Address = address
            //};
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO users VALUES (@User_ID, @First_Name, @Last_Name, @Gender, @Birth_Date, @HMO_ID, @Blood_Type, @Address) ", conn);
                cmd.Parameters.AddWithValue("@User_ID", User_ID);
                cmd.Parameters.AddWithValue("@First_Name", First_Name);
                cmd.Parameters.AddWithValue("@Last_Name", Last_Name);
                cmd.Parameters.AddWithValue("@Gender", Gender);
                DateTime dt = Convert.ToDateTime(Birth_Date);
                cmd.Parameters.AddWithValue("@Birth_Date", dt);
                cmd.Parameters.AddWithValue("@HMO_ID", HMO_ID);
                cmd.Parameters.AddWithValue("@Blood_Type", Blood_Type);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
               
                if (isDoctor==true)
                    permissionID = 3;
                else
                    permissionID = 2;

                MySqlCommand cmd2 = new MySqlCommand("INSERT INTO premission_for_users VALUES (@Prem_ID, @User_ID, @User_Name, @PW_Hash)", conn);
                cmd2.Parameters.AddWithValue("@Prem_ID", permissionID);
                cmd2.Parameters.AddWithValue("@User_ID", User_ID);
                cmd2.Parameters.AddWithValue("@User_Name", username);

                var sha512 = SHA512.Create();
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");
                
                cmd2.Parameters.AddWithValue("@PW_Hash", hashedPassword);
                cmd2.ExecuteNonQuery();
                cmd2.Parameters.Clear();
            }
           // return newUser;
        }

        private int GenerateAutoID()
        {
            int i;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select Count (User_ID) from users", conn);
                i = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                i++;
            }
            return i;
        }

        public List<blood_test> GetUserTests(int userId)
        {
            List<blood_test> testsForUser = new List<blood_test>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from blood_test WHERE BUser_ID=@User_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        testsForUser.Add(new blood_test()
                        {
                            BTestID = reader.GetInt32("BTest_ID"),
                            BUserID = reader.GetInt32("BUser_ID"),
                            BTestDate = reader.GetDateTime("BTest_Date"),
                            //BDocName = reader.GetString("BDoc_Name")
                        });
                    }
                }
            }
            return testsForUser;
        }

        public List<blood_test_data> GetTestResultByID (int testId)
        {
            List<blood_test_data> bloodTestResult = new List<blood_test_data>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from blood_test_data WHERE BTest_ID=@TestID", conn);
                cmd.Parameters.AddWithValue("@TestID", testId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bloodTestResult.Add(new blood_test_data()
                        {
                            BTestID = reader.GetInt32("BTest_ID"),
                            BCompID = reader.GetInt32("BComp_ID"),
                            Value = reader.GetDouble("Value")
                        });
                    }
                }
            }
            return bloodTestResult;
        }

        public void GetSOMByID(int SOM_ID)
        {
            supplements_or_medication_info SOMI = new supplements_or_medication_info();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM supplements_or_medication_info WHERE SOM_ID=@SOM_ID", conn);
                cmd.Parameters.AddWithValue("@SOM_ID", SOM_ID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SOMI.SomID = reader.GetInt32("SOM_ID");
                        SOMI.PharmID = reader.GetInt32("Pharm_ID");
                        SOMI.SOMName = reader.GetString("SOM_NAME");
                        SOMI.ServingAmount = reader.GetInt32("Serving_Amount");
                        SOMI.ProductCode = reader.GetString("Product_Code");
                        Enum.TryParse(reader.GetString("Code_Type"), out CodeType CT);
                        SOMI.CodeType = CT;
                        Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                        SOMI.InHealthPlan = IHP;
                        Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                        SOMI.WithMedicalPrescription = WMP;
                    }
                }
            }
        }
    }
}
