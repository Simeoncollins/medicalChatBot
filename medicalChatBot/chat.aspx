<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="chat.aspx.cs" Inherits="medicalChatBot.chat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AI Medical Assitant</title>
    <link href="style.css" rel="stylesheet" />
    <link rel="stylesheet" href="StyleSheet.css"/>
        <script src="jquery-3.6.0.js"></script>
    <style>
        body *{
            transition: all 0.3s ease;
        }
        header {
    display: flex;
    width: 200px;
    border-radius: 4px;
}

        header nav {
            flex-direction: column;
        }

        .logo {
    text-align: center;
}

        .nav-links {
    text-align: center;
}

        .myBtn {
    border: 1px solid;
    padding: 7px;
    border-radius: 5px;
    margin-bottom: 10px;
    transition: all 0.3s;
}

        .myBtn:hover{
            background: white;
            border-color: black;
            color: black;
        }
    </style>
    <script type="importmap">
        {
            "imports": {
                "@google/generative-ai": "https://esm.run/@google/generative-ai"
            }
        }
    </script>
</head>
<body class="h-full font-sans antialiased">
    <form id="form1" runat="server">
    <div>
      <div>
          
        <div class="dark" id="mainCotain">
          <main class="flex h-full w-full flex-1 flex-row items-stretch bg-zinc-50 dark:bg-zinc-800">
        <header>
        <nav>
            <div class="logo">
                AI Healthcare Assistant Powered By Gemini AI
            </div>
            <ul class="nav-links">
                <li><button type="button" id="change" class="myBtn">Change Theme</button></li>
                <li>
                    <asp:Button Text="Logout" runat="server" ID="logoutBtn" CssClass="myBtn" OnClick="logoutBtn_Click"/></li>
            </ul>
        </nav>
    </header>
            <div class="mx-auto flex h-[100vh] w-full flex-col">
              <div class="mx-auto flex h-[100vh] w-full max-w-7xl flex-col gap-4 px-4 pb-4">
                <div class="flex w-full flex-row items-center justify-center gap-4 rounded-b-xl bg-zinc-200 px-4 py-2 dark:bg-zinc-700">
              <div class="mr-auto flex h-full items-center">
                <div class="flex items-center rounded-xl">
                            <div class="prose prose-base prose-zinc max-w-full dark:prose-invert">
                                <p id="welcome" runat="server">Welcome, Simeon Collins</p>
                            </div>
                        </div>
              </div>
              
            </div>
                <div id="chatBox" runat="server" class="flex-1 overflow-y-auto scroll-smooth rounded-xl bg-zinc-100 p-4 text-sm leading-6 text-zinc-900 sm:text-base sm:leading-7 dark:bg-zinc-800 dark:text-zinc-300">
                  
                </div>
                  <div class="flex rounded-xl bg-zinc-50 px-2 py-6 sm:px-4 dark:bg-zinc-900 thinkingTxt" style="display:none">
                      <img class="mr-2 flex h-10 w-10 rounded-full border border-gray-200 bg-white object-contain sm:mr-4" src="images/ai.jpg">
                      <div class="flex items-center rounded-xl">
                          <div class="prose prose-base prose-zinc max-w-full dark:prose-invert prose-headings:font-semibold prose-h1:text-lg prose-h2:text-base prose-h3:text-base prose-p:first:mt-0 prose-a:text-blue-600 prose-code:text-sm prose-code:text-white prose-pre:p-2">
                              <div>
                                  <p><i>thinking...</i></p>
                              </div>
                          </div>
                      </div>
                  </div>
                <div class="mt-2">
                  <div class="relative">
                      <textarea id="inputText"
                      class="block max-h-[500px] w-full resize-none rounded-xl border-none bg-zinc-100 p-4 pl-4 pr-20 text-sm text-zinc-900 focus:outline-none focus:ring-2 focus:ring-blue-500 sm:text-base dark:bg-zinc-700 dark:text-zinc-200 dark:placeholder-zinc-400 dark:focus:ring-blue-500"
                      placeholder="Enter your prompt" style="height: 56px;"></textarea>
                      <button type="button" id="sendRequest"
                      class="group absolute bottom-2 right-2.5 flex h-10 w-10 items-center justify-center rounded-lg bg-blue-700 text-sm font-medium text-zinc-200 transition duration-200 ease-in-out hover:bg-blue-800 focus:outline-none focus:ring-4 focus:ring-blue-300 disabled:opacity-50 sm:text-base dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800"><!--v-if--><svg
                        xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none"
                        stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" size="20"
                        class="tabler-icon tabler-icon-send">
                        <path d="M10 14l11 -11"></path>
                        <path d="M21 3l-6.5 18a.55 .55 0 0 1 -1 0l-3.5 -7l-7 -3.5a.55 .55 0 0 1 0 -1l18 -6.5"></path>
                      </svg></button></div>
                </div>
              </div>
            </div>
          </main>
        </div>
          
      </div>
        <input type="hidden" name="login" value="no" id="login" runat="server" />
        <input type="hidden" name="imageSrc" id="imageSrc" value="" runat="server"/>
        <input type="hidden" name="id" id="conversationID" value="" runat="server"/>
        <input type="hidden" name="id" id="pastConversation" value="" runat="server"/>
        <script>
            $("#change").on("click", function () {
                $("#mainCotain").toggleClass("dark");
            })
        </script>
        <script type="module">
    import { GoogleGenerativeAI } from "@google/generative-ai";

    const API_KEY = 'YOUR-GEMINI-API-KEY';
    const genAI = new GoogleGenerativeAI(API_KEY);
    const model = genAI.getGenerativeModel({ model: "gemini-2.5-flash" });
    var imageUrl = $("#imageSrc").val();
    var conversationID = $("#conversationID").val(); // Gets the conversationID from the hidden input
    var conversationHistory = $("#pastConversation").val(); // Gets the conversation history from the hidden input

    $(document).ready(function () {
            $("#chatBox").scrollTop($("#chatBox")[0].scrollHeight);
        // On login, append the "hi" message to conversation history and insert the message into the DB
        if ($("#login").val() == "yes") {
            conversationHistory += "User: hi \n";
            updateConversation(conversationHistory, conversationID); // Update conversation history
            // Fetch AI response based on the conversation history
        model.generateContent(conversationHistory).then(result => {
            const aiResponse = result.response.text();
            conversationHistory += "AI: " + aiResponse + "\n";
            appendMessage('ai', aiResponse);
            insertMessage(aiResponse, conversationID, "ai"); // Insert AI message
            updateConversation(conversationHistory, conversationID); // Update conversation history
        }).catch(error => {
            console.error('Error:', error);
            appendMessage('ai', 'Sorry, an error occurred. Please try again.');
        });
        }

        

        // Handle send button click
        $('#sendRequest').click(function () {
            $(".thinkingTxt").show();
            sendMessage();
        });

        // Handle enter key press for message sending
        $('#inputText').keypress(function (e) {
            if (e.which === 13) {
                sendMessage();
            }
        });

        function sendMessage() {
            var inputText = $('#inputText').val().trim();

            if (!inputText) {
                alert('Please type a message!');
                return;
            }

            appendMessage('user', inputText);
            $('#inputText').val(''); // Clear input field after sending
            conversationHistory += "User: " + inputText + "\n";

            // Insert user message into DB
            insertMessage(inputText, conversationID, "user");

            // Update conversation in DB
            updateConversation(conversationHistory, conversationID);

            // Fetch AI response for the updated conversation history
            model.generateContent(conversationHistory).then(result => {
                const aiResponse = result.response.text();
                conversationHistory += "AI: " + aiResponse + "\n";

                $(".thinkingTxt").hide();
                appendMessage('ai', aiResponse);

                // Insert AI response into DB
                insertMessage(aiResponse, conversationID, "ai");

                // Update conversation in DB
                updateConversation(conversationHistory, conversationID);
            }).catch(error => {
                console.error('Error:', error);
                appendMessage('ai', 'Sorry, an error occurred. Please try again.');
            });
        }

        // Function to append message to chat box
        function appendMessage(sender, message) {
            var chatBox = $('#chatBox');
            var messageElement = "";

            if (sender === 'user') {
                messageElement = `
                    <div class="flex flex-row px-2 py-4 sm:px-4">
                        <img class="mr-2 flex h-10 w-10 rounded-full sm:mr-4" src="${imageUrl}"/>
                        <div class="flex max-w-3xl items-center">
                            <div class="prose prose-base prose-zinc max-w-full dark:prose-invert prose-headings:font-semibold prose-h1:text-lg prose-h2:text-base prose-h3:text-base prose-p:first:mt-0 prose-a:text-blue-600 prose-code:text-sm prose-code:text-white prose-pre:p-2">
                                <div>
                                    <p>${message}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            } else {
                messageElement = `
                    <div class="flex rounded-xl bg-zinc-50 px-2 py-6 sm:px-4 dark:bg-zinc-900">
                        <img class="mr-2 flex h-10 w-10 rounded-full border border-gray-200 bg-white object-contain sm:mr-4" src="images/ai.jpg">
                        <div class="flex items-center rounded-xl">
                            <div class="prose prose-base prose-zinc max-w-full dark:prose-invert prose-headings:font-semibold prose-h1:text-lg prose-h2:text-base prose-h3:text-base prose-p:first:mt-0 prose-a:text-blue-600 prose-code:text-sm prose-code:text-white prose-pre:p-2">
                                <div>
                                    <p>${message}</p>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            }

            chatBox.append(messageElement);
            chatBox.scrollTop(chatBox[0].scrollHeight);
        }

        // Ajax function to insert a new message into the DB
        function insertMessage(messageText, conversationID, sender) {
            $.ajax({
                type: "POST",
                url: "chat.aspx/insertMessage", // Adjust this URL as per your project
                data: JSON.stringify({ messageText: messageText, conversationID: conversationID, sender: sender }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    console.log('Message inserted successfully');
                },
                error: function (error) {
                    console.error('Error inserting message:', error);
                }
            });
        }

        // Ajax function to update the conversation history in the DB
        function updateConversation(conversationHistory, conversationID) {
            $.ajax({
                type: "POST",
                url: "chat.aspx/updateConversation", // Adjust this URL as per your project
                data: JSON.stringify({ messageText: conversationHistory, conversationID: conversationID }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    console.log('Conversation updated successfully');
                },
                error: function (error) {
                    console.error('Error updating conversation:', error);
                }
            });
        }
    });
        </script>

    </div>
    </form>
</body>
</html>
