<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="signup.aspx.cs" Inherits="medicalChatBot.signup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
        <meta charset="UTF-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Patient Registration</title>
    <link rel="stylesheet" href="StyleSheet.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css"/>
    <script src="jquery-3.6.0.js"></script>
    <style>
        header{
            margin-bottom: 43px;
        }
        footer{
            margin-top: 60px;
        }
        .red
        {
            color: Red;   
         }
         
         .green
        {
            color: green;   
         }
         
         .disabled
        {
            pointer-events: none;
            opacity: 0.5;
         }
         
         #submitBtn
         {
             cursor: pointer;
             }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <header>
        <nav>
            <div class="logo">
                <i class="fas fa-robot"></i> AI Healthcare Assistant
            </div>
            <ul class="nav-links">
                <li><a href="login.aspx">Login</a></li>
            </ul>
        </nav>
    </header>

    <section class="registration-form">
        <div class="form-container">
            <h1>Patient Registration</h1>
            <div>
                <div class="input-group">
                    <label for="fullName"><i class="fas fa-user"></i> Full Name</label>
                    <input type="text" id="fullName" name="fullName" required runat="server"/>
                </div>

                <div class="input-group">
                    <label for="email"><i class="fas fa-envelope"></i> Email</label>
                    <input type="email" id="email" name="email" required runat="server"/>
                </div>

                <div class="input-group">
                    <label for="gender"><i class="fas fa-venus-mars"></i> Gender</label>
                    <select id="gender" name="gender" required runat="server">
                        <option value="none">Select Gender</option>
                        <option value="male">Male</option>
                        <option value="female">Female</option>
                    </select>
                </div>

                <div class="input-group">
                    <label for="dob"><i class="fas fa-calendar-alt"></i> Date of Birth</label>
                    <input type="date" id="dob" name="dob" required runat="server"/>
                </div>
                
                <div class="input-group">
                    <label for="faceImage"><i class="fas fa-medkit"></i> Detailed Medical History</label>
                    <span style="color: #34495e; font-size: 12px; font-weight: bold;">may include medical conditions,  recent symptoms, medication, allergies, past surgeries, family medical history e.t.c, please itemize or list out for better clarification.</span>
                    <textarea id="history" name="history" required runat="server" rows="5" cols="10"></textarea>
                </div>
                
                <div class="input-group">
                    <label for="password"><i class="fas fa-lock"></i> Password</label>
                    <input type="password" id="password" name="password" required runat="server" />
                </div>

                <div class="input-group">
                    <label for="confirmPassword"><i class="fas fa-lock"></i> Confirm Password</label>
                    <input type="password" id="confirmP" name="confirmPassword" required runat="server"/>
                </div>
                <p id="res"></p>
                <asp:Button Text="Register" runat="server" ID="submitBtn" CssClass="btn disabled" OnClick="submitBtn_ServerClick"/>
            </div>
        </div>
    </section>

    <footer>
        <p>&copy; 2024 AI Healthcare Assistant System. All rights reserved.</p>
    </footer>
         <script type="text/javascript">
            $("#password").on("keyup", validate)
            $("#confirmP").on("keyup", validate)
            function validate() {
                var pass = $("#password").val();
                var confirm = $("#confirmP").val();

                if (pass !== confirm) {
                    $("#res").text("password does not match!");
                    $("#res").addClass("red");
                    $("#res").removeClass("green");
                    $("#submitBtn").addClass("disabled");
                }

                else {
                    $("#res").text("password match!");
                    $("#res").addClass("green");
                    $("#res").removeClass("red");
                    $("#submitBtn").removeClass("disabled");
                }
            }

            $("#submitBtn").on("click", function (e) {
                if ($("#gender").val() == "none") {
                    e.preventDefault();
                    alert("Please select a gender");
                    $("#gender").focus();
                }
            })
         </script>
    </div>
    </form>
</body>
</html>
