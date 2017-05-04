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

        public List <health_maintenance_organizations> GetAllHMO()
        {
            List<health_maintenance_organizations> HMOList = new List<health_maintenance_organizations>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from health_maintenance_organizations", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HMOList.Add(new health_maintenance_organizations()
                        {
                            HMOID = reader.GetInt32("HMO_ID"),
                            HMOName = reader.GetString("HMO_NAME")
                            //BDocName = reader.GetString("BDoc_Name")
                        });
                    }
                }
            }
            return HMOList;
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

        //public List<blood_component> GetTestResultByID (int testId)
        //{
        //    List<blood_test_data> bloodTestResult = new List<blood_test_data>();
        //    List<blood_component> bloodComponents = new List<blood_component>();

        //    using (MySqlConnection conn = GetConnection())
        //    {
        //        conn.Open();
                
        //        MySqlCommand cmd = new MySqlCommand("Select * from blood_test_data WHERE BTest_ID=@TestID", conn);
        //        cmd.Parameters.AddWithValue("@TestID", testId);
        //        using (MySqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                bloodTestResult.Add(new blood_test_data()
        //                {
        //                    BTestID = reader.GetInt32("BTest_ID"),
        //                    BCompID = reader.GetInt32("BComp_ID"),
        //                    Value = reader.GetDouble("Value")
        //                });
        //            }
        //        }

        //        MySqlCommand cmd2 = new MySqlCommand("Select * FROM blood_component INNER JOIN blood_test_data WHERE blood_test_data.BTest_ID=@TestID and blood_test_data.BComp_ID=blood_component.BComp_ID", conn);
        //        cmd2.Parameters.AddWithValue("@TestID", testId);
        //        //MySqlCommand cmd2 = new MySqlCommand("Select * from blood_component", conn);
        //        using (MySqlDataReader reader = cmd2.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                bloodComponents.Add(new blood_component()
        //                {
        //                    BCompID = reader.GetInt32("BComp_ID"),
        //                    BCompName = reader.GetString("BComp_Name"),
        //                    MeasurementUnit = reader.GetString("Measurement_Unit"),
        //                    MenMax = reader.GetDouble("Men_Max"),
        //                    MenMin = reader.GetDouble("Men_Min"),
        //                    WomenMax = reader.GetDouble("Women_Max"),
        //                    WomenMin = reader.GetDouble("Women_Min")
        //                });
        //            }
        //        }
        //    }
        //    return bloodComponents;
        //}

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

        public int GetIDByUserName(string userName)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from premission_for_users WHERE User_Name=@User_Name", conn);
                cmd.Parameters.AddWithValue("@User_Name", userName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                        return reader.GetInt32("User_ID");
                }
            }
        }

        public List<BloodTestViewModel> GetTestResultByID(int testId)
        {
            BloodTestViewModel BTVM = new BloodTestViewModel();
            List <BloodTestViewModel> BTVMList = new List <BloodTestViewModel>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("Select * FROM blood_test INNER JOIN blood_test_data INNER JOIN blood_or_additive_component INNER JOIN users WHERE blood_test_data.BTest_ID=@TestID and blood_test_data.BComp_ID=blood_or_additive_component.BComp_ID and users.User_ID=blood_test.BUser_ID", conn);
                cmd.Parameters.AddWithValue("@TestID", testId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BTVM.user.UserID = reader.GetInt32("User_ID");
                        BTVM.user.Address = reader.GetString("Address");
                        BTVM.user.BirthDate = Convert.ToDateTime(reader.GetString("Birth_Date"));
                        BTVM.user.FirstName = reader.GetString("First_Name");
                        BTVM.user.LastName = reader.GetString("Last_Name");
                        Enum.TryParse(reader.GetString("Gender"), out Gender gender);
                        BTVM.user.Gender = gender;
                        Enum.TryParse(reader.GetString("Blood_Type"), out BloodType BT);
                        BTVM.user.BloodType = BT;
                        BTVM.user.HMOID = reader.GetInt32("HMO_ID");

                        BTVM.bloodTest.BTestID = reader.GetInt32("BTest_ID");
                        BTVM.bloodTest.BUserID = reader.GetInt32("BUser_ID");
                        //BTVM.bloodTest.DoctorName = reader.GetInt32("Doctor_Name");
                        BTVM.bloodTest.BTestDate = Convert.ToDateTime(reader.GetString("BTest_Date"));
                        Enum.TryParse(reader.GetString("IsPregnant"), out IsPregnant IP);
                        BTVM.bloodTest.IsPregnant = IP;

                        BTVM.btData.BCompID = reader.GetInt32("BComp_ID");
                        BTVM.btData.BTestID = reader.GetInt32("BTest_ID");
                        BTVM.btData.Value = reader.GetDouble("Value");

                        BTVM.BOAComp.BOA_ID = reader.GetInt32("BOA_ID");
                        BTVM.BOAComp.BOA_Name = reader.GetString("BOA_Name");
                        BTVM.BOAComp.info = reader.GetString("Info");
                        BTVM.BOAComp.MeasurementUnit = reader.GetString("Measurement_Unit");
                        BTVM.BOAComp.MenMax = reader.GetDouble("Men_Max");
                        BTVM.BOAComp.MenMin = reader.GetDouble("Men_Min");
                        BTVM.BOAComp.PregnantMax = reader.GetDouble("Pregnant_Max");
                        BTVM.BOAComp.PregnantMin = reader.GetDouble("Pregnant_Min");
                        BTVM.BOAComp.WomenMax = reader.GetDouble("Women_Max");
                        BTVM.BOAComp.WomenMin = reader.GetDouble("Women_Min");

                        BTVMList.Add(BTVM);
                    }
                }
            }
            return BTVMList;
        }
    }
}
