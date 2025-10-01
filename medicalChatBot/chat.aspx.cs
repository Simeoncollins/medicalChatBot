using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace medicalChatBot
{
    public partial class chat : System.Web.UI.Page
    {
        
        string patientid;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["email"] == null)
            {
                Response.Redirect("default.aspx");
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
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT fullname FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SQLiteDataReader dr = cmd.ExecuteReader();
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
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT id FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SQLiteDataReader dr = cmd.ExecuteReader();
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

            using (SQLiteConnection conn = DatabaseInitializer.GetConnection())
            {
                string query = "SELECT * FROM Messages WHERE ConversationID = @ConversationID ORDER BY timestamp ASC";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ConversationID", conversID);
                    conn.Open();

                    SQLiteDataReader reader = cmd.ExecuteReader();

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
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM conversations WHERE patientID = @patientID", con))

                {
                    cmd.Parameters.AddWithValue("@patientID", patientid);
                    con.Open();
                    SQLiteDataReader dr = cmd.ExecuteReader();
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
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT photoUrl FROM patients WHERE email = @email", con))

                {
                    cmd.Parameters.AddWithValue("@email", Session["email"].ToString());
                    con.Open();
                    SQLiteDataReader dr = cmd.ExecuteReader();
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
        public static bool insertMessage(string messageText, string conversationID, string sender)
        {
            if (string.IsNullOrWhiteSpace(messageText) ||
                string.IsNullOrWhiteSpace(conversationID) ||
                string.IsNullOrWhiteSpace(sender))
            {
                return false;
            }

            try
            {
                DateTime currentUtcTime = DateTime.UtcNow; // Use UTC
                using (SQLiteConnection con = DatabaseInitializer.GetConnection())
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO messages (conversationID, sender, messageText, timestamp) VALUES (@conversationID, @sender, @text, @timestamp)", con))
                    {
                        cmd.Parameters.AddWithValue("@conversationID", conversationID);
                        cmd.Parameters.AddWithValue("@sender", sender);
                        cmd.Parameters.AddWithValue("@text", messageText.Trim());
                        cmd.Parameters.AddWithValue("@timestamp", currentUtcTime.ToString("yyyy-MM-dd HH:mm:ss")); // Store as UTC
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting message: {ex.Message}");
                return false;
            }
        }


        [WebMethod]
        public static void updateConversation(string messageText, string conversationID)
        {
            
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE conversations SET text = @text WHERE conversationID = @conversationID", con))
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
            Response.Redirect("default.aspx");
        }
    }
}