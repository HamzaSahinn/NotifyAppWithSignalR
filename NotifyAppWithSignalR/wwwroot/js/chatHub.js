
class Manager {
    constructor() {
        const self = this;
        this.reciverId = "";
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/signalhub")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        self.connection.onclose(async () => {
            await self.connectSignalRHub();
        });

        $(".sideBar-body").on("click", function (e) {
            self.changeReciver(e);
        });

        $("#sendButton").on("click", (e) => {
            var message = $("#messageInput").val();
            if (message) {
                $("#conversation").append(self.createSentMessage({ body:message, createdDate: Date.now() }));
                self.connection.invoke("SendPrivate", self.reciverId, message);
                $("#conversation").animate({ scrollTop: $("#conversation").height() }, 100);
                $("#messageInput").val("")
            }
        });

        self.connection.on("MessageRecived", (userId, name, message, time) => {
            if (userId === self.reciverId) {
                $("#conversation").append(self.createRecivedMessage({ body: message, createdDate: time }));
                $("#conversation").animate({ scrollTop: $("#conversation").height() }, 100);
            }
            else {
                var div = self.findOrCreateUserDiv(userId, name, time);
                $(div).find(".notificaitonIcon").show();
            }
        });

        $("#searchText").on("typeahead:select", (e, datum, dbName) => {
            self.reciverId = datum.val;
            var div = self.findOrCreateUserDiv(datum.val, datum.display, Date.now());
            div.trigger("click");
        });

        this.connectSignalRHub();
    }

    async changeReciver(e) {
        this.reciverId = $(e.target).closest(".sideBar-body").data("reciverid");
        var messageHistory = await this.getMessageHistory();
        $("#currentReciverName").html($(e.target).closest(".sideBar-body").find(".name-meta").first().html());
        $("#reciverAvatar").html($(e.target).closest(".sideBar-body").find(".avatar-icon").first().html());
        this.updateMessageHistory(true, messageHistory);
        $("#conversation").animate({ scrollTop: $("#conversation").height() }, 100);
        $(e.target).closest(".sideBar-body").find(".notificaitonIcon").hide();

    }

    async connectSignalRHub() {
        try {
            await this.connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(this.connectSignalRHub, 5000);
        }
    }

    async getMessageHistory() {
        var response = await fetch(`message/history/${this.reciverId}`);
        var data = await response.json();

        return data;
    }

    createRecivedMessage(item) {
        return $(`<div class="row message-body">
            <div class="col-sm-12 message-main-receiver">
                <div class="receiver">
                    <div class="message-text">
                        ${item.body}
                    </div>
                    <span class="message-time pull-right">
                        ${item.createdDate}
                    </span>
                </div>
            </div>
        </div>`);
    }

    createSentMessage(item) {
        return $(`<div class="row message-body">
            <div class="col-sm-12 message-main-sender">
                <div class="sender">
                    <div class="message-text">
                        ${item.body}
                    </div>
                    <span class="message-time pull-right">
                        ${item.createdDate}
                    </span>
                </div>
            </div>
        </div>`);
    }

    updateMessageHistory(isNewConversation, data) {
        var messageBlog;
        if (isNewConversation) {
            $("#conversation").empty();
        }
        var self = this;
        data.forEach((item) => {
            messageBlog = item.isSender ? self.createSentMessage(item.message) :self.createRecivedMessage(item.message) ;
            $("#conversation").prepend(messageBlog);
        });
    }

    findOrCreateUserDiv(userId, name, time) {
        var self = this;
        var userDiv = $(`.sideBar-body[data-reciverid="${userId}"]`).first();
        if (userDiv.length === 0) {
            userDiv = $(`<div class="row sideBar-body" data-reciverid="${userId}">
                    <div class="col-sm-3 col-xs-3 sideBar-avatar">
                        <div class="avatar-icon">
                            <img src="https://ui-avatars.com/api/?name=${encodeURIComponent(name)}">
                        </div>
                    </div>
                    <div class="col-sm-9 col-xs-9 sideBar-main">
                        <div class="row">
                            <div class="col-sm-7 col-xs-7 sideBar-name">
                                <span class="name-meta">
                                    ${name}
                                </span>
                            </div>
                           <div class="col-sm-3 col-xs-3 pull-right sideBar-time">
                                    <span class="time-meta pull-right">
                                    </span>
                                </div>
                                <div class="col-sm-2 col-xs-2 notificaitonIcon" style="display:none">
                                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" style="width:24px">
                                        <path stroke-linecap="round" stroke-linejoin="round" d="M8.625 12a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H8.25m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H12m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0h-.375M21 12c0 4.556-4.03 8.25-9 8.25a9.764 9.764 0 0 1-2.555-.337A5.972 5.972 0 0 1 5.41 20.97a5.969 5.969 0 0 1-.474-.065 4.48 4.48 0 0 0 .978-2.025c.09-.457-.133-.901-.467-1.226C3.93 16.178 3 14.189 3 12c0-4.556 4.03-8.25 9-8.25s9 3.694 9 8.25Z" />
                                    </svg>
                                </div>
                        </div>
                    </div>
                </div>`);
        }

        $(userDiv).on("click", function (e) {
            self.changeReciver(e);
        });

        return userDiv;
    }
}

var manager;

$(document).ready(function () {
    manager = new Manager();

    var bestUsers = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: '/Home/Users?term=%QUERY',
            wildcard: '%QUERY'
        }
    });

    $('#searchText').typeahead(null, {
        name: 'best-users',
        display: 'display',
        source: bestUsers
    });
});