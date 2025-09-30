using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace medicalChatBot
{
    public partial class chat : System.Web.UI.Page
    {
        string constring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
        string patientid;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["email"] == null)
            {
                Response.Redirect("login.aspx");
            }

            if (Session["CameFromLogin"] != null && (bool)Session["CameFromLogin"] == true)
            {
                login.Value = "yes";
                Session["CameFromLogin"] = false;
            }
            patientid = getID();
            getWelcome();
            imageSrc.Value = getImage();
            getCoversation();
            if (!IsPostBack)
            {
                displayMessages();
            }
        }

        private void getWelcome()
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT fullname FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        welcome.InnerText = "Welcome, " + dr["fullname"].ToString();
                    }
                    con.Close();
                }
            }
        }

        private string getID()
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return dr["id"].ToString();
                    }
                    con.Close();
                }
            }
            return "";
        }

        private void displayMessages()
        {
            string conversID = conversationID.Value;
            chatBox.InnerHtml = "";

            using (SqlConnection conn = new SqlConnection(constring))
            {
                string query = "SELECT * FROM Messages WHERE ConversationID = @ConversationID ORDER BY timestamp ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ConversationID", conversID);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string sender = reader["sender"].ToString().Trim();
                        string messageText = reader["messageText"].ToString().Trim();
                        string messageHtml = "";

                        if (sender == "user")
                        {
                            messageHtml = @"
                    <div class='flex flex-row px-2 py-4 sm:px-4 mb-4'>
                        <img class='mr-2 flex h-10 w-10 rounded-full sm:mr-4' src='" + getImage() + @"'/>
                        <div class='flex max-w-3xl items-center'>
                            <div class='prose prose-base prose-zinc max-w-full dark:prose-invert'>
                                <p>" + messageText + @"</p>
                            </div>
                        </div>
                    </div>";
                        }
                        else if (sender == "ai")
                        {
                            messageHtml = @"
                    <div class='flex rounded-xl bg-zinc-50 px-2 py-6 sm:px-4 dark:bg-zinc-900 mb-4'>
                        <img class='mr-2 flex h-10 w-10 rounded-full border border-gray-200 bg-white object-contain sm:mr-4' src='images/ai.jpg'>
                        <div class='flex items-center rounded-xl'>
                            <div class='prose prose-base prose-zinc max-w-full dark:prose-invert'>
                                <p>" + messageText + @"</p>
                            </div>
                        </div>
                    </div>";
                        }

                        chatBox.InnerHtml += messageHtml;
                    }

                    conn.Close();
                }
            }
        }


        private void getCoversation()
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM conversations WHERE patientID = @patientID", con))

                {
                    cmd.Parameters.AddWithValue("@patientID", patientid);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        conversationID.Value = dr["conversationID"].ToString();
                        pastConversation.Value = dr["text"].ToString();
                    }
                    con.Close();
                }
            }
        }

        private string getImage()
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT photoUrl FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return dr["photoUrl"].ToString();
                    }
                    con.Close();
                }
            }
            return "";
        }

        [WebMethod]
        public static void insertMessage(string messageText, string conversationID, string sender)
        {
            string constring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            DateTime currentLocalTime = DateTime.Now.ToLocalTime();
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO messages (conversationID, sender, messageText, timestamp) VALUES (@conversationID, @sender, @text, @timestamp)", con))
                {
                    cmd.Parameters.AddWithValue("@conversationID", conversationID);
                    cmd.Parameters.AddWithValue("@sender", sender);
                    cmd.Parameters.AddWithValue("@text", messageText);
                    cmd.Parameters.AddWithValue("@timestamp", currentLocalTime.ToString());
                    cmd.ExecuteNonQuery();
                }
            }
        }


        [WebMethod]
        public static void updateConversation(string messageText, string conversationID)
        {
            string constring = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constring))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE conversations SET text = @text WHERE conversationID = @conversationID", con))
                {
                    cmd.Parameters.AddWithValue("@conversationID", conversationID);
                    cmd.Parameters.AddWithValue("@text", messageText);
                    cmd.ExecuteNonQuery();
                }
            }
        }



        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session["email"] = null;
            Response.Redirect("login.aspx");
        }
    }
}