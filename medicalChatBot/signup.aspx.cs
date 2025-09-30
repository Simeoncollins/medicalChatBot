using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace medicalChatBot
{
    public partial class signup : System.Web.UI.Page
    {
        string constring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        protected void submitBtn_ServerClick(object sender, EventArgs e)
        {
            validateEmail();
        }
        private void validateEmail()
        {
            string mail = email.Value.Trim();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE email = @email", con))
                {
                    cmd.Parameters.AddWithValue("@email", mail);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Response.Write("<script>alert('A patient with this email is already registered!')</script>");
                            email.Focus();
                        }
                        else
                        {
                            registerUser();
                        }

                    }
                }
            }
        }

        private void registerUser()
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO patients (fullname, email, dob, gender, medicalHistory, password, photoUrl) OUTPUT INSERTED.id VALUES (@fullname, @email, @dob, @gender, @medicalHistory, @password, @photoUrl)", con))
                {
                    DateTime dateObject = DateTime.Parse(dob.Value);
                    cmd.Parameters.AddWithValue("@fullname", fullName.Value.Trim());
                    cmd.Parameters.AddWithValue("@email", email.Value.Trim());
                    cmd.Parameters.AddWithValue("@dob", dateObject.ToShortDateString());
                    cmd.Parameters.AddWithValue("@gender", gender.Value.Trim());
                    cmd.Parameters.AddWithValue("@medicalHistory", history.Value.Trim());
                    cmd.Parameters.AddWithValue("@password", password.Value.Trim());
                    cmd.Parameters.AddWithValue("@photoUrl", "/images/avatar.png");
                    int newPatientId = (int)cmd.ExecuteScalar();
                    insertConversation(newPatientId);

                    Response.Write("<script>alert('Your registration is successful!');");
                    Response.Write("window.location='login.aspx'</script>");
                }
            }

        }

        private void insertConversation(int newPatientId)
        {
            string instructions = @"You are a highly intelligent healthcare assistant designed to provide real - time healthcare assistance,
              improve patient engagement, and enhance the efficiency of healthcare delivery. Your role involves
              understanding and responding to a wide range of medical queries with the following capabilities:
            -Provide accurate and personalized medical advice based on patient symptoms, medical history, and needs.
            - Offer preliminary diagnoses, advise on treatments, and recommend when to consult a healthcare provider.
            - Set reminders for medications, explaining dosage and side effects.
            - Provide educational resources related to conditions, prevention, and general health.
            - Respond in real time, adapt to evolving conditions, and recognize emergencies.
            - Communicate empathetically and ethically, respecting patient privacy.

            You understand your role is to assist and not replace a healthcare provider.Always remind users to consult a qualified professional for final diagnoses and treatments.";
            string history = getPatientHealthHistory();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO conversations VALUES (@patientID, @text)", con))
                {
                    DateTime dateObject = DateTime.Parse(dob.Value);
                    cmd.Parameters.AddWithValue("@patientID", newPatientId);
                    cmd.Parameters.AddWithValue("@text", instructions + "\n\n" + history + "\n\n");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string getPatientHealthHistory()
        {
            DateTime dateOfBirth = DateTime.Parse(dob.Value);
            int year = Convert.ToInt32(DateTime.Today.Year);
            int birthYear = Convert.ToInt32(dateOfBirth.Year);
            int age = year - birthYear;
            string fullname = fullName.Value.Trim();
            string sex = gender.Value.Trim();
            string healthHistory = history.Value.Trim();

            string patientHealthHistory = @"This patient, " + fullname + @" has the following health history:
                                      - Age: " + age + @" -gender: " + sex + @" -health information: " + healthHistory +
                                          @"\n past conversations with you (when reading the past conversation with you, note that the text prefixed with 'AI:' were messages from you while the ones prefixed with 'User:' where messages from the patient, make your conversation be a continuation from this but if there's nothing after this, know that you have not had any previous conversation with this patient. 
                                      also note, Do not respond to this message, respsond only to the messages after this and when responding, do not manually prefix the texts and don not respond to meesages sent by yourself or respond to messages on behalf of the user. ): ";

            return patientHealthHistory;
        }
    }
}