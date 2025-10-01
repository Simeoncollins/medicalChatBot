using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace medicalChatBot
{
    public partial class login : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void submitBtn_ServerClick(object sender, EventArgs e)
        {
            string mail = email.Value.Trim();
            string passwordTxt = password.Value.Trim();
            using (SQLiteConnection con = DatabaseInitializer.GetConnection())
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM patients WHERE email = @email AND @password = password", con))
                {
                    cmd.Parameters.AddWithValue("@email", email.Value);
                    cmd.Parameters.AddWithValue("@password", passwordTxt);
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Session["email"] = mail;
                            Session["CameFromLogin"] = true;
                            Response.Redirect("chat.aspx");
                        }
                        else
                        {
                            Response.Write("<script>alert('invalid credentials')</script>");
                            email.Focus();
                        }
                    }
                }
            }
        }
    }
}