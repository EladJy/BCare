﻿using System;
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
                conn.Close();
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
                conn.Close();
            }
            return false;
        }

        public void Register(int User_ID, string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string username, string password, bool isDoctor)
        {
            int permissionID=1;
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
                conn.Close();
            }
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

        public long CountTestsByID(int User_ID)
        {
            long count;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(BTest_ID) FROM blood_test WHERE BUser_ID=@User_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", User_ID);
                count = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close(); 
            }
            return count;
        }

        public List<supplements_or_medication_info> TopFiveMedications()
        {
            List<supplements_or_medication_info> SOMIList = new List<supplements_or_medication_info>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM supplements_or_medication_info LIMIT 5", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enum.TryParse(reader.GetString("Code_Type"), out CodeType CT);
                        Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                        Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                        SOMIList.Add(new supplements_or_medication_info()
                        {
                            SomID = reader.GetInt32("SOM_ID"),
                            PharmID = reader.GetInt32("Pharm_ID"),
                            SOMName = reader.GetString("SOM_Name"),
                            ServingAmount= reader.GetInt32("Serving_Amount"),
                            ProductCode= reader.GetString("Product_Code"),   
                            CodeType = CT,                
                            InHealthPlan = IHP,                 
                            WithMedicalPrescription = WMP

                    });
                    }
                }
                conn.Close();
            }
            return SOMIList;
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
                        });
                    }
                }
                conn.Close();
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
                        Enum.TryParse(reader.GetString("IsPregnant"), out IsPregnant IP);
                        testsForUser.Add(new blood_test()
                        {
                            BTestID = reader.GetInt32("BTest_ID"),
                            BUserID = reader.GetInt32("BUser_ID"),
                            BTestDate = reader.GetDateTime("BTest_Date"),
                            DoctorName = reader.GetString("Doctor_Name"),
                            IsPregnant = IP
                        });
                    }
                }
                conn.Close();
            }
            return testsForUser;
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
                conn.Close();
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
                    reader.Read();
                    return reader.GetInt32("User_ID");
                }
                conn.Close();
            }
        }

        public User GetUserDetailsByID(int User_ID)
        {
            User user = new User();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from users WHERE User_ID=@User_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", User_ID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) {  
                        user.UserID = reader.GetInt32("User_ID");
                        user.FirstName = reader.GetString("First_Name");
                        user.LastName = reader.GetString("Last_Name");
                        Enum.TryParse(reader.GetString("Gender"), out Gender gender);
                        user.Gender = gender;
                        user.Address = reader.GetString("Address");
                        user.BirthDate = Convert.ToDateTime(reader.GetString("Birth_Date"));
                        Enum.TryParse(reader.GetString("Blood_Type"), out BloodType BT);
                        user.BloodType = BT;
                        user.HMOID = reader.GetInt32("HMO_ID");
                    }
                }
                conn.Close();
            }
            return user;
        }

        public void UpdateUserDetails(int User_ID, string firstName, string lastName, string Gender, string birth, int HMOID, string bloodType, string Address, string userName, string pwd, string Email)
        {
            User user = new User();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                if ((firstName.Length != 0)&&(lastName.Length!=0))
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE users SET First_Name = @First_Name, Last_Name = @Last_Name, Gender = @Gender, Birth_date=@Birth_Date, HMO_ID=@HMO_ID, Blood_Type=@Blood_Type, Address=@Address WHERE User_ID =@User_ID ", conn);
                    cmd.Parameters.AddWithValue("@User_ID", User_ID);
                    cmd.Parameters.AddWithValue("@First_Name", firstName);
                    cmd.Parameters.AddWithValue("@Last_Name", lastName);
                    cmd.Parameters.AddWithValue("@Gender", Gender);
                    cmd.Parameters.AddWithValue("@Birth_Date", Convert.ToDateTime(birth));
                    cmd.Parameters.AddWithValue("@HMO_ID", HMOID);
                    cmd.Parameters.AddWithValue("@Blood_Type", bloodType);
                    cmd.Parameters.AddWithValue("@Address", Address);
                    cmd.ExecuteNonQuery();

                    var sha512 = SHA512.Create();
                    byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");
                    MySqlCommand cmd2 = new MySqlCommand("UPDATE premission_for_users SET User_Name = @userName, PW_Hash = @hashedPassword, Email = @Email WHERE User_ID =@User_ID ", conn);
                    cmd2.Parameters.AddWithValue("@User_ID", User_ID);
                    cmd2.Parameters.AddWithValue("@userName", userName);
                    cmd2.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                    cmd2.Parameters.AddWithValue("@Email", Email);
                    cmd2.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public List<BloodTestViewModel> GetTestResultByID(int testId)
        {   
            List <BloodTestViewModel> BTVMList = new List <BloodTestViewModel>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(" FROM (((blood_test INNER JOIN users ON blood_test.BUser_ID=users.User_ID) INNER JOIN blood_test_data ON blood_test_data.BTest_ID=blood_test.BTest_ID) INNER JOIN blood_or_additive_component ON blood_or_additive_component.BOA_ID=blood_test_data.BComp_ID) WHERE blood_test.BTest_ID=@TestID", conn);
                cmd.Parameters.AddWithValue("@TestID", testId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BloodTestViewModel BTVM = new BloodTestViewModel();
                        Enum.TryParse(reader.GetString("Blood_Type"), out BloodType BT);
                        Enum.TryParse(reader.GetString("Gender"), out Gender gender);
                        BTVM.user = new User()
                        {
                            UserID = reader.GetInt32("User_ID"),
                            Address = reader.GetString("Address"),
                            BirthDate = Convert.ToDateTime(reader.GetString("Birth_Date")),
                            FirstName = reader.GetString("First_Name"),
                            LastName = reader.GetString("Last_Name"),
                            Gender = gender,
                            BloodType = BT,
                            HMOID = reader.GetInt32("HMO_ID")
                        };
                        Enum.TryParse(reader.GetString("IsPregnant"), out IsPregnant IP);
                        BTVM.bloodTest = new blood_test()
                        {
                            BTestID = reader.GetInt32("BTest_ID"),
                            BUserID = reader.GetInt32("BUser_ID"),
                            DoctorName = reader.GetString("Doctor_Name"),
                            BTestDate = Convert.ToDateTime(reader.GetString("BTest_Date")),
                            IsPregnant = IP
                        };


                        BTVM.btData = new blood_test_data()
                        {
                            BCompID = reader.GetInt32("BComp_ID"),
                            BTestID = reader.GetInt32("BTest_ID"),
                            Value = reader.GetDouble("Value")
                        };

                        BTVM.BOAComp = new blood_or_additive_component()
                        {
                            BOA_ID = reader.GetInt32("BOA_ID"),
                            BOA_Name = reader.GetString("BOA_Name"),
                            info = reader.GetString("Info"),
                            MeasurementUnit = reader.GetString("Measurement_Unit"),
                            MenMax = reader.GetDouble("Men_Max"),
                            MenMin = reader.GetDouble("Men_Min"),
                            PregnantMax = reader.GetDouble("Pregnant_Max"),
                            PregnantMin = reader.GetDouble("Pregnant_Min"),
                            WomenMax = reader.GetDouble("Women_Max"),
                            WomenMin = reader.GetDouble("Women_Min")
                        };
                        BTVMList.Add(BTVM);
                    }
                }
                conn.Close();
            }
            return BTVMList;
        }
    }
}
