<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="medicalChatBot.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
       <meta charset="UTF-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Patient Login</title>
    <link rel="stylesheet" href="StyleSheet.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css"/>
    <style>
        header{
            margin-bottom: 43px;
        }
        footer{
            margin-top: 60px;
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
                <li><a href="signup.aspx">Signup</a></li>
            </ul>
        </nav>
    </header>

    <section class="registration-form">
        <div class="form-container">
            <h1>Patient Login</h1>
            <div>
                <div class="input-group">
                    <label for="matricNumber"><i class="fas fa-id-card"></i> Email</label>
                    <input type="email" id="email" name="email" required runat="server"/>
                </div>

                
                <div class="input-group">
                    <label for="password"><i class="fas fa-lock"></i> Password</label>
                    <input type="password" id="password" name="password" required runat="server"/>
                </div>

                <button runat="server" id="submitBtn" onserverclick="submitBtn_ServerClick" class="btn">Login <i class="fas fa-arrow-right"></i></button>
            </div>
        </div>
    </section>

    <footer>
        <p>&copy; 2024 AI Healthcare Assistant System. All rights reserved.</p>
    </footer>
    </div>
    </form>
</body>
</html>
