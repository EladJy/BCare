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
                conn.Close();
            }

            return users;
        }
        public bool Login(string username, string password)
        {
            User user = new User();
            using (MySqlConnection conn = GetConnection())
            {

                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (username.Equals(reader.GetString("User_Name")))
                        {
                            var sha512 = SHA512.Create();
                            byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                            string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");
                            if (hashedPassword.Equals(reader.GetString("PW_Hash")))
                            {
                                return true;
                            }
                        }
                    }
                }
                conn.Close();
            }
            return false;
        }

        public void Register(int User_ID, string First_Name, string Last_Name, string Gender, string Birth_Date, int HMO_ID, string Blood_Type, string Address, string username, string password, string email, bool isDoctor)
        {
            string permissionUser = "User";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO users VALUES (@User_ID, @First_Name, @Last_Name, @Gender, @Birth_Date, @HMO_ID, @Blood_Type, @Address,@Premission_Name, @User_Name, @PW_Hash,@Email) ", conn);
                cmd.Parameters.AddWithValue("@User_ID", User_ID);
                cmd.Parameters.AddWithValue("@First_Name", First_Name);
                cmd.Parameters.AddWithValue("@Last_Name", Last_Name);
                cmd.Parameters.AddWithValue("@Gender", Gender);
                DateTime dt = Convert.ToDateTime(Birth_Date);
                cmd.Parameters.AddWithValue("@Birth_Date", dt);
                cmd.Parameters.AddWithValue("@HMO_ID", HMO_ID);
                cmd.Parameters.AddWithValue("@Blood_Type", Blood_Type);
                cmd.Parameters.AddWithValue("@Address", Address);

                if (isDoctor == true)
                    permissionUser = "Doctor";
                else
                    permissionUser = "User";

                cmd.Parameters.AddWithValue("@Premission_Name", permissionUser);
                cmd.Parameters.AddWithValue("@User_Name", username);
                cmd.Parameters.AddWithValue("@Email", email);

                var sha512 = SHA512.Create();
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");

                cmd.Parameters.AddWithValue("@PW_Hash", hashedPassword);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

        public double GetEffectOnComp(int boaID , int somID)
        {
            double effect = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT ace.Effect FROM active_component_effect_in_med ace WHERE ace.ACEM_BOA_ID = @Boa_ID AND ace.ACEM_SOM_ID = @Som_ID", conn);
                cmd.Parameters.AddWithValue("@Boa_ID", boaID);
                cmd.Parameters.AddWithValue("@Som_ID", somID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        effect = reader.GetDouble("Effect");
                    }
                }
                conn.Close();
            }
            return effect;
        }

        public List<active_component_effect_in_med> getAllEffects()
        {
            List<active_component_effect_in_med> aceList = new List<active_component_effect_in_med>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM active_component_effect_in_med", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        aceList.Add(new active_component_effect_in_med()
                        {
                            ACEM_ATC_ID = reader.GetString("ACEM_ATC_ID"),
                            ACEM_BOA_ID = reader.GetInt32("ACEM_BOA_ID"),
                            ACEM_SOM_ID = reader.GetInt32("ACEM_SOM_ID"),
                            Effect = reader.GetInt32("Effect")
                        });
                    }
                }
                conn.Close();
            }

            return aceList;
        }
        public blood_test GetBloodTestByID(int testID)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                blood_test bt = new blood_test();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM blood_test WHERE BTest_ID=@BTest_ID", conn);
                cmd.Parameters.AddWithValue("@BTest_ID", testID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Enum.TryParse(reader.GetString("IsPregnant"), out IsPregnant IP);
                    bt.BTestID = reader.GetInt32("BTest_ID");
                    bt.BUserID = reader.GetInt32("BUser_ID");
                    bt.DoctorName = reader.GetString("Doctor_Name");
                    bt.BTestDate = Convert.ToDateTime(reader.GetString("BTest_Date"));
                    bt.IsPregnant = IP;
                }
                conn.Close();
                return bt;
            }

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
                        if (reader["MoreInformation"] != DBNull.Value && reader["ProductImage_URL"] != DBNull.Value)
                        {
                            Enum.TryParse(reader.GetString("Serving_Form_Type"), out ServingType ST);
                            Enum.TryParse(reader.GetString("Serving_Form_Unit"), out MeasurementUnit SU);
                            Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                            Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                            SOMIList.Add(new supplements_or_medication_info()
                            {
                                SomID = reader.GetInt32("SOM_ID"),
                                PharmID = reader.GetInt32("Pharm_ID"),
                                SOMName = reader.GetString("SOM_Name"),
                                ServingAmountInBox = reader.GetInt32("Serving_Amount_In_Box"),
                                ServingFormType = ST,
                                ServingFormUnit = SU,
                                InHealthPlan = IHP,
                                WithMedicalPrescription = WMP,
                                MoreInformation = reader.GetString("MoreInformation"),
                                ProductImageURL = reader.GetString("ProductImage_URL")
                            });
                        }
                        else
                        {
                            Enum.TryParse(reader.GetString("Serving_Form_Type"), out ServingType ST);
                            Enum.TryParse(reader.GetString("Serving_Form_Unit"), out MeasurementUnit SU);
                            Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                            Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                            SOMIList.Add(new supplements_or_medication_info()
                            {
                                SomID = reader.GetInt32("SOM_ID"),
                                PharmID = reader.GetInt32("Pharm_ID"),
                                SOMName = reader.GetString("SOM_Name"),
                                ServingAmountInBox = reader.GetInt32("Serving_Amount_In_Box"),
                                ServingFormType = ST,
                                ServingFormUnit = SU,
                                InHealthPlan = IHP,
                                WithMedicalPrescription = WMP,
                                MoreInformation = "abc",
                                ProductImageURL = "abc"
                            });
                        }

                    }
                }
                conn.Close();
            }
            return SOMIList;
        }

        public List<health_maintenance_organizations> GetAllHMO()
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

        public List<Tuple<DateTime, double>> CompValuesStats(int userID, int compID)
        { // values of component by tests
            List<Tuple<DateTime, double>> testValuesByCompId = new List<Tuple<DateTime, double>>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT Value,DATE(BTest_Date) FROM blood_test_data INNER JOIN blood_test WHERE BUser_ID=@User_ID AND blood_test_data.BTest_ID = blood_test.BTest_ID AND BComp_ID = @Comp_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", userID);
                cmd.Parameters.AddWithValue("@Comp_ID", compID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        testValuesByCompId.Add(Tuple.Create(reader.GetDateTime("DATE(BTest_Date)"), reader.GetDouble("Value")));
                    }
                    conn.Close();
                }
            }
            return testValuesByCompId;
        }
        public List<Tuple<string, int>> countUsersByBloodTypeStats()
        {
            List<Tuple<string, int>> countBloodType = new List<Tuple<string, int>>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(User_ID),Blood_Type FROM users GROUP BY Blood_Type ORDER BY COUNT(User_ID) DESC", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countBloodType.Add(Tuple.Create(reader.GetString("Blood_Type"), reader.GetInt32("COUNT(User_ID)")));
                    }
                    conn.Close();
                }
            }
            return countBloodType;
        }

        public List<Tuple<string, int>> UserBloodTestByDateStats(int userID) // number of blood test per year
        {
            List<Tuple<string, int>> countBloodTests = new List<Tuple<string, int>>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(BUser_ID), YEAR(BTest_Date) as Year FROM blood_test WHERE blood_test.BUser_ID=314118456 GROUP BY Year ORDER BY YEAR(BTest_Date) ASC", conn);
                cmd.Parameters.AddWithValue("@User_ID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countBloodTests.Add(Tuple.Create(reader.GetString("Year"), reader.GetInt32("COUNT(BUser_ID)")));
                    }
                    conn.Close();
                }
            }
            return countBloodTests;
        }

        public presCommentViewModel getPrescriptionDetails(int presID, int bloodID)
        {
            presCommentViewModel commentsAndPres = new presCommentViewModel();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT bt.BTest_Date FROM blood_test bt WHERE bt.BTest_ID = @bloodID", conn);
                cmd.Parameters.AddWithValue("@bloodID", bloodID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        commentsAndPres.bloodTest_Date = reader.GetDateTime("BTest_Date");
                    }

                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM prescription_details pd INNER JOIN supplements_or_medication_info somi ON pd.PDSOM_ID = somi.SOM_ID where pd.PDPres_ID = @Pres_ID", conn);
                cmd2.Parameters.AddWithValue("@Pres_ID", presID);
                using (MySqlDataReader reader = cmd2.ExecuteReader())
                {
                    commentsAndPres.somcList = new List<SOMConsumeViewModel>();

                    while (reader.Read())
                    {
                        Enum.TryParse(reader.GetString("Serving_Form_Type"), out ServingType ST);
                        Enum.TryParse(reader.GetString("Serving_Form_Unit"), out MeasurementUnit SU);
                        Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                        Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                        commentsAndPres.somcList.Add(new SOMConsumeViewModel
                        {
                            pres = new prescription_details
                            {
                                AmountToConsume = reader.GetInt32("Amount_To_Consume_Per_Day"),
                                DaysToConsume = reader.GetInt32("Days_To_Consume"),
                                PDPresID = reader.GetInt32("PDPres_ID"),
                                PDSom_ID = reader.GetInt32("PDSOM_ID"),
                                Text = reader.GetString("Text")
                            },

                            SOMI = new supplements_or_medication_info
                            {
                                SomID = reader.GetInt32("SOM_ID"),
                                PharmID = reader.GetInt32("Pharm_ID"),
                                SOMName = reader.GetString("SOM_Name"),
                                ServingAmountInBox = reader.GetInt32("Serving_Amount_In_Box"),
                                ServingFormType = ST,
                                ServingFormUnit = SU,
                                InHealthPlan = IHP,
                                WithMedicalPrescription = WMP,
                                MoreInformation = reader.GetString("MoreInformation"),
                                ProductImageURL = reader.GetString("ProductImage_URL")
                            }
                        });
                    }
                }
                MySqlCommand cmd3 = new MySqlCommand("SELECT rof.* , u.First_Name , u.Last_Name FROM review_or_feedback rof INNER JOIN users u ON rof.RFUser_ID= u.User_ID WHERE rof.RFPres_ID = @Pres_ID", conn);
                cmd3.Parameters.AddWithValue("@Pres_ID", presID);
                using (MySqlDataReader reader = cmd3.ExecuteReader())
                {
                    commentsAndPres.rofvmList = new List<review_or_feedback_ViewModel>();
                    while (reader.Read())
                    {
                        Enum.TryParse(reader.GetString("rating"), out Rating rate);
                        commentsAndPres.rofvmList.Add(new review_or_feedback_ViewModel
                        {
                            rof = new review_or_feedback
                            {
                                Rating = rate,
                                ReviewDate = reader.GetDateTime("Review_Date"),
                                RFPresID = reader.GetInt32("RFPres_ID"),
                                RFSomID = reader.GetInt32("RFSOM_ID"),
                                RFUserID = reader.GetInt32("RFUser_ID"),
                                Text = reader.GetString("Text")
                            },
                            first_Name = reader.GetString("First_Name"),
                            last_Name = reader.GetString("Last_Name")
                        });
                    }

                }
                conn.Close();
            }
            return commentsAndPres;
        }

        public string getComponentInfo(int boaID)
        {
            string info = "";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT BOA_Name , boa.Info from blood_or_additive_component boa WHERE boa.BOA_ID = @boa_ID", conn);
                cmd.Parameters.AddWithValue("@boa_ID", boaID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        info = "<h2 style=\"direction: ltr; text-align:right;\">" + reader.GetString("BOA_Name") + "</h2>" + reader.GetString("Info");
                    }
                    conn.Close();
                }
            }
            return info;
        }
        public List<Tuple<string, int>> countUsersByHMOStats()
        {
            List<Tuple<string, int>> countHMO = new List<Tuple<string, int>>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(User_ID),HMO_NAME FROM users INNER JOIN health_maintenance_organizations WHERE users.HMO_ID=health_maintenance_organizations.HMO_ID GROUP BY health_maintenance_organizations.HMO_ID ORDER BY COUNT(User_ID) DESC", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countHMO.Add(Tuple.Create(reader.GetString("HMO_NAME"), reader.GetInt32("COUNT(User_ID)")));
                    }
                    conn.Close();
                }
            }
            return countHMO;
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

        public bool isAdmin(int userId)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT Premission_Name FROM users WHERE User_ID=@UserID", conn);
                cmd.Parameters.AddWithValue("@userID", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Enum.TryParse(reader.GetString("Premission_Name"), out PremissionName PN);
                    if (PN.ToString().Equals("Admin"))
                    {
                        return true;
                    }
                    else
                        return false;
                }
            }
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
                        SOMI.ServingAmountInBox = reader.GetInt32("Serving_Amount_In_Box");
                        Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                        SOMI.InHealthPlan = IHP;
                        Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                        SOMI.WithMedicalPrescription = WMP;
                        SOMI.MoreInformation = reader.GetString("MoreInformation");
                        Enum.TryParse(reader.GetString("Serving_Form_Type"), out ServingType ST);
                        SOMI.ServingFormType = ST;
                        Enum.TryParse(reader.GetString("Serving_Form_Unit"), out MeasurementUnit SU);
                        SOMI.ServingFormUnit = SU;
                    }
                }
                conn.Close();
            }
        }

        public string GetUserNameByID(int id)
        {
            string userName;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select User_Name from users WHERE User_ID=@User_Name", conn);
                cmd.Parameters.AddWithValue("@User_ID", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    userName = reader.GetString("User_Name");
                }
                conn.Close();
                return userName;
            }
        }

        public int GetIDByUserName(string userName)
        {
            int ID;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select * from users WHERE User_Name=@User_Name", conn);
                cmd.Parameters.AddWithValue("@User_Name", userName);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    ID = reader.GetInt32("User_ID");
                }
                conn.Close();
                return ID;
            }
        }

        public int GetPresByBloodTest(int bloodTestID)
        {
            int ID = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select Pres_ID from prescription WHERE PBTest_ID=@PBTest_ID", conn);
                cmd.Parameters.AddWithValue("@PBTest_ID", bloodTestID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        ID = reader.GetInt32("Pres_ID");
                    }
                }
                conn.Close();
                return ID;
            }
        }

        public List<supplements_or_medication_info> GetAllMed()
        {
            List<supplements_or_medication_info> listMed = new List<supplements_or_medication_info>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * from supplements_or_medication_info", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enum.TryParse(reader.GetString("Serving_Form_Type"), out ServingType ST);
                        Enum.TryParse(reader.GetString("Serving_Form_Unit"), out MeasurementUnit SU);
                        Enum.TryParse(reader.GetString("In_Health_Plan"), out InHealthPlan IHP);
                        Enum.TryParse(reader.GetString("With_Medical_Prescription"), out WithMedicalPrescription WMP);
                        listMed.Add(new supplements_or_medication_info()
                        {
                            SomID = reader.GetInt32("SOM_ID"),
                            PharmID = reader.GetInt32("Pharm_ID"),
                            SOMName = reader.GetString("SOM_Name"),
                            ServingAmountInBox = reader.GetInt32("Serving_Amount_In_Box"),
                            ServingFormType = ST,
                            ServingFormUnit = SU,
                            InHealthPlan = IHP,
                            WithMedicalPrescription = WMP,
                            MoreInformation = "abc",
                            ProductImageURL = "abc"
                        });
                    }
                }
                conn.Close();
            }
            return listMed;
        }
        public UserDetailViewModel GetUserDetailsByID(int User_ID)
        {
            UserDetailViewModel userDetails = new UserDetailViewModel();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE users.User_ID=@User_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", User_ID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Enum.TryParse(reader.GetString("Blood_Type"), out BloodType BT);
                        Enum.TryParse(reader.GetString("Gender"), out Gender gender);
                        Enum.TryParse(reader.GetString("Premission_Name"), out PremissionName premissionName);
                        userDetails.user = new User()
                        {
                            UserID = reader.GetInt32("User_ID"),
                            FirstName = reader.GetString("First_Name"),
                            LastName = reader.GetString("Last_Name"),
                            Gender = gender,
                            Address = reader.GetString("Address"),
                            BirthDate = Convert.ToDateTime(reader.GetString("Birth_Date")),
                            BloodType = BT,
                            HMOID = reader.GetInt32("HMO_ID"),
                            Email = reader.GetString("Email"),
                            PremissionType = premissionName,
                            PWHash = reader.GetString("PW_Hash"),
                            UserName = reader.GetString("User_Name")
                        };
                    }
                    if (userDetails.user.PremissionType.ToString().Equals("Doctor"))
                    {
                        userDetails.isDoctor = true;
                    }
                    else
                    {
                        userDetails.isDoctor = false;
                    }
                }
                conn.Close();
            }
            return userDetails;
        }


        public void UpdateUserDetails(int User_ID, string firstName, string lastName, string Gender, string birth, int HMOID, string bloodType, string Address, string userName, string pwd, string Email, bool isDoctor)
        {
            string permissionUser = "User";
            User user = new User();
            bool isAdministrator = isAdmin(User_ID);
            if (isAdministrator)
            {
                permissionUser = "Admin";
            }
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                if ((firstName.Length != 0) && (lastName.Length != 0) && pwd != null)
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE users SET First_Name = @First_Name, Last_Name = @Last_Name,Premission_Name = @permissionUser , Gender = @Gender, Birth_date=@Birth_Date, HMO_ID=@HMO_ID, Blood_Type=@Blood_Type, Address=@Address. User_Name = @userName, PW_Hash = @hashedPassword, Email = @Email WHERE @UserID =users.User_ID ", conn);
                    cmd.Parameters.AddWithValue("@UserID", User_ID);
                    cmd.Parameters.AddWithValue("@First_Name", firstName);
                    cmd.Parameters.AddWithValue("@Last_Name", lastName);
                    cmd.Parameters.AddWithValue("@Gender", Gender);
                    cmd.Parameters.AddWithValue("@Birth_Date", Convert.ToDateTime(birth));
                    cmd.Parameters.AddWithValue("@HMO_ID", HMOID);
                    cmd.Parameters.AddWithValue("@Blood_Type", bloodType);
                    cmd.Parameters.AddWithValue("@Address", Address);
                    if (isDoctor == true && !isAdministrator)
                        permissionUser = "Doctor";
                    cmd.Parameters.AddWithValue("@permissionUser", permissionUser);
                    var sha512 = SHA512.Create();
                    byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                    string hashedPassword = BitConverter.ToString(bytes).Replace("-", "");
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE users SET First_Name = @First_Name, Last_Name = @Last_Name,Premission_Name = @permissionUser ,Gender = @Gender, Birth_date=@Birth_Date, HMO_ID=@HMO_ID, Blood_Type=@Blood_Type, Address=@Address, User_Name = @userName, Email = @Email WHERE @UserID =users.User_ID ", conn);
                    cmd.Parameters.AddWithValue("@UserID", User_ID);
                    cmd.Parameters.AddWithValue("@First_Name", firstName);
                    cmd.Parameters.AddWithValue("@Last_Name", lastName);
                    cmd.Parameters.AddWithValue("@Gender", Gender);
                    cmd.Parameters.AddWithValue("@Birth_Date", Convert.ToDateTime(birth));
                    cmd.Parameters.AddWithValue("@HMO_ID", HMOID);
                    cmd.Parameters.AddWithValue("@Blood_Type", bloodType);
                    cmd.Parameters.AddWithValue("@Address", Address);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    if (isDoctor == true && !isAdministrator)
                        permissionUser = "Doctor";
                    cmd.Parameters.AddWithValue("@permissionUser", permissionUser);
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public void SetNewComment(int UserID, int SomID, int PresID, string date, int rating, string text)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO review_or_feedback VALUES (@User_ID, @Som_ID, @Pres_ID, @Com_Date, @Com_Rating, @Com_Content) ", conn);
                cmd.Parameters.AddWithValue("@User_ID", UserID);
                cmd.Parameters.AddWithValue("@Som_ID", SomID);
                cmd.Parameters.AddWithValue("@Pres_ID", PresID);
                DateTime dt = Convert.ToDateTime(date);
                cmd.Parameters.AddWithValue("@Com_Date", dt);
                cmd.Parameters.AddWithValue("@Com_Rating", rating);
                cmd.Parameters.AddWithValue("@Com_Content", text);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

       

        public void UpdateComment(int UserID, int SomID, int PresID, string date, int rating, string text)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("UPDATE review_or_feedback SET Review_Date=@Com_Date, rating=@Com_Rating, Text=@Com_Content  WHERE RFUser_ID=@User_ID AND RFSOM_ID=@Som_ID AND RFPres_ID=@Pres_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", UserID);
                cmd.Parameters.AddWithValue("@Som_ID", SomID);
                cmd.Parameters.AddWithValue("@Pres_ID", PresID);
                DateTime dt = Convert.ToDateTime(date);
                cmd.Parameters.AddWithValue("@Com_Date", dt);
                cmd.Parameters.AddWithValue("@Com_Rating", rating);
                cmd.Parameters.AddWithValue("@Com_Content", text);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

        public void DeleteComment(int UserID, int SomID, int PresID)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM review_or_feedback WHERE RFUser_ID=@User_ID AND RFSOM_ID=@Som_ID AND RFPres_ID=@Pres_ID", conn);
                cmd.Parameters.AddWithValue("@User_ID", UserID);
                cmd.Parameters.AddWithValue("@Som_ID", SomID);
                cmd.Parameters.AddWithValue("@Pres_ID", PresID);
          
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

        public double GetAvgRatingBySOMID(int SOM_ID)
        {
            double avg = 5;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT AVG(rating) as avgRating FROM review_or_feedback WHERE RFSOM_ID=@SOM_ID", conn);
                cmd.Parameters.AddWithValue("SOM_ID", SOM_ID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    if (reader.HasRows)
                    {
                        if(reader["avgRating"] != DBNull.Value)
                        {
                            avg = reader.GetDouble("avgRating");
                        }
                    }
                    cmd.Parameters.Clear();
                }
                conn.Close();
            }
            return avg;
        }

        public List<blog> getAllPostsInBlog()
        {
            List <blog> blogPosts= new List<blog>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM blog", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blogPosts.Add(new blog ()
                        { 
                        PostID = reader.GetInt32("Post_ID"),
                        PostAuthorID = reader.GetInt32("Post_Author"),
                        PostDate = reader.GetDateTime("Post_Date"),
                        PostContent = reader.GetString("Post_Content"),
                        PostTitle = reader.GetString("Post_Title"),
                        PostModified = reader.GetDateTime("Post_Modified")
                        });
                    }
                }
                conn.Close();
            }
            return blogPosts;
        }

        public blog getPostByID (int PostID)
        {
            blog BLOG = new blog();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM blog WHERE Post_ID=@PostID ", conn);
                cmd.Parameters.AddWithValue("PostID", PostID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BLOG.PostID = reader.GetInt32("Post_ID");
                        BLOG.PostAuthorID = reader.GetInt32("Post_Author");
                        BLOG.PostDate = reader.GetDateTime("Post_Date");
                        BLOG.PostContent = reader.GetString("Post_Content");
                        BLOG.PostTitle= reader.GetString("Post_Title");
                        BLOG.PostModified = reader.GetDateTime("Post_Modified");
                    }
                }
                conn.Close();
            }
            return BLOG;
        }

        public void SetNewPost(int PostID, int authorID, string date, string content, string title, string dateModify)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO blog VALUES (@Post_ID, @Post_Author, @Post_Date, @Post_Content, @Post_Title, @Post_Modified) ", conn);
                cmd.Parameters.AddWithValue("@Post_ID", PostID);
                cmd.Parameters.AddWithValue("@Post_Author", authorID);
                DateTime dt = Convert.ToDateTime(date);
                cmd.Parameters.AddWithValue("@Post_Date", dt);
                cmd.Parameters.AddWithValue("@Post_Content", content);
                cmd.Parameters.AddWithValue("@Post_Title", title);
                DateTime dt2 = Convert.ToDateTime(dateModify);
                cmd.Parameters.AddWithValue("@Post_Modified", dt2);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }
        public void UpdatePost(int PostID, int authorID, string date, string content, string title, string dateModify)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Update blog SET Post_Content=@Post_Content, Post_Title=@Post_Title, Post_Modified=@Post_Modified WHERE Post_ID=@Post_ID AND Post_Author=@Post_Author", conn);
                cmd.Parameters.AddWithValue("@Post_ID", PostID);
                cmd.Parameters.AddWithValue("@Post_Author", authorID);
                DateTime dt = Convert.ToDateTime(date);
                cmd.Parameters.AddWithValue("@Post_Date", dt);
                cmd.Parameters.AddWithValue("@Post_Content", content);
                cmd.Parameters.AddWithValue("@Post_Title", title);
                DateTime dt2 = Convert.ToDateTime(dateModify);
                cmd.Parameters.AddWithValue("@Post_Modified", dt2);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

        public void DeletePost(int Post_ID)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM blog WHERE Post_ID=@Post_ID", conn);
                cmd.Parameters.AddWithValue("@Post_ID", Post_ID);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
            }
        }

        public BloodTestViewModel GetTestResultByID(int testId)
        {
            BloodTestViewModel BTVM = new BloodTestViewModel();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT b.BUser_ID , b.Doctor_Name , b.BTest_Date , b.IsPregnant FROM blood_test b WHERE b.BTest_ID = @TestID", conn);
                cmd.Parameters.AddWithValue("@TestID", testId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BTVM.User_ID = reader.GetInt32("BUser_ID");
                        BTVM.Doctor_Name = reader.GetString("Doctor_Name");
                        BTVM.BT_Date = reader.GetDateTime("BTest_Date");
                        BTVM.IsPregnant = reader.GetString("IsPregnant");
                    }
                }
                MySqlCommand cmd2 = new MySqlCommand("SELECT * FROM blood_test_data btd INNER JOIN blood_or_additive_component boa ON btd.BComp_ID = boa.BOA_ID WHERE btd.BTest_ID = @TestID", conn);
                cmd2.Parameters.AddWithValue("@TestID", testId);
                using (MySqlDataReader reader = cmd2.ExecuteReader())
                {
                    BTVM.BTC = new List<BloodTestCompnentViewModel>();
                    while (reader.Read())
                    {
                        BTVM.BTC.Add(new BloodTestCompnentViewModel
                        {
                            btData = new blood_test_data
                            {
                                Value = reader.GetInt32("Value"),
                                BCompID = reader.GetInt32("BComp_ID"),
                                BTestID = reader.GetInt32("BTest_ID")
                            },
                            BOAComp = new blood_or_additive_component
                            {
                                BOA_ID = reader.GetInt32("BOA_ID"),
                                BOA_Name = reader.GetString("BOA_Name"),
                                //info = reader.GetString("Info"),
                                MeasurementUnit = reader.GetString("Measurement_Unit"),
                                MenMax = reader.GetInt32("Men_Max"),
                                MenMin = reader.GetInt32("Men_Min"),
                                PregnantMax = reader.GetInt32("Pregnant_Max"),
                                PregnantMin = reader.GetInt32("Pregnant_Min"),
                                WomenMax = reader.GetInt32("Women_Max"),
                                WomenMin = reader.GetInt32("Women_Min"),
                            }
                        });
                    }
                }
                MySqlCommand cmd3 = new MySqlCommand("SELECT u.Gender FROM users u WHERE u.User_ID = @User_ID", conn);
                cmd3.Parameters.AddWithValue("@User_ID", BTVM.User_ID);
                using (MySqlDataReader reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BTVM.UserGender = reader.GetString("Gender");
                    }
                }
                conn.Close();
            }
            return BTVM;
        }
    }
}