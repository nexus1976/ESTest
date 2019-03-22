'use strict';
angular.module('cb').controller('cb-comment-board-controller', ['$scope', '$timeout', '$document', '$q', 'cb-data-service', function ($scope, $timeout, $document, $q, cbDataService) {
    $.connection.hub.url = apiURL + '/signalr';
    $scope.vm.hub = $.connection.conversationHub;
    $scope.vm.activeConversations = [];
    $scope.vm.selectedConversation = {};
    $scope.vm.chatLog = [];
    $scope.vm.hubId = null;
    $scope.vm.messageText = '';
    $scope.vm.userId = userId;

    $scope.vm.getActiveConversations = function () {
        cbDataService.getActiveConversations().then(
            function successCallback(response) {
                if (response && response.data) {
                    $scope.vm.activeConversations = response.data;
                } else {
                    $scope.vm.activeConversations = [];
                }
            },
            function errorCallback(response) {
                console.log(response);
            }
        );
    };
    $scope.vm.getSelectedConversation = function (conversationId) {
        cbDataService.getConversationDetails(conversationId).then(
            function successCallBack(response) {
                if (response && response.data) {
                    $scope.vm.selectedConversation = response.data;
                    $scope.vm.selectedConversation.open = true;
                    $scope.vm.chatLog.push({ MessageText: 'joined room: ' + $scope.vm.selectedConversation.Topic });
                    if ($scope.vm.hubId !== $scope.vm.selectedConversation.ConversationId) {
                        if ($scope.vm.hubId) {
                            $scope.vm.hub.server.unsubscribe($scope.vm.hubId, $scope.vm.userId);
                        }
                        $scope.vm.hubId = $scope.vm.selectedConversation.ConversationId;
                        $scope.vm.hub.server.subscribe($scope.vm.hubId, $scope.vm.userId);
                    }
                    //$scope.vm.getSelectedConversationMessages(conversationId);
                } else {
                    $scope.vm.selectedConversation = {};
                    if ($scope.vm.hubId) {
                        $scope.vm.hub.server.unsubscribe($scope.vm.hubId, $scope.vm.userId);
                        $scope.vm.hubId = null;
                    }
                }
            },
            function errorCallback(response) {
                console.log(response);
                $scope.vm.selectedConversation = {};
                if ($scope.vm.hubId) {
                    $scope.vm.hub.server.unsubscribe($scope.vm.hubId, $scope.vm.userId);
                    $scope.vm.hubId = null;
                }
            }
        );
    };
    $scope.vm.getSelectedConversationMessages = function (conversationId) {
        cbDataService.getLastSeveralMessages(conversationId).then(
            function successCallBack(response) {
                if (response && response.data) {
                    console.log('last 5 messages: ', response.data);
                    var len = response.data.length,
                        i = 0;
                    for (; i < len; i++) {
                        $scope.vm.chatLog.push(response.data[i]);
                    }
                }
            },
            function errorCallback(response) {
                console.log(response);
            }
        );
    };
    $scope.vm.selectConversation = function (conversation) {
        $timeout(function () {
            $scope.vm.selectedConversation.open = conversation.open;
            if ($scope.vm.selectedConversation) {
                if ($scope.vm.selectConversation.ConversationId !== conversation.ConversationId) {
                    $scope.vm.leaveUserFromConversation();
                }
            }
            if (conversation.open) {
                $scope.vm.chatLog = [];
                $scope.vm.getSelectedConversation(conversation.ConversationId);
            } else {
                $scope.vm.leaveUserFromConversation();
            }
        }, 0, true);
    };
    $scope.vm.leaveUserFromConversation = function () {
        if ($scope.vm.hubId) {
            $scope.vm.hub.server.unsubscribe($scope.vm.hubId, $scope.vm.userId);
            $scope.vm.hubId = null;
        }
    };
    $scope.vm.addMessage = function () {
        if ($scope.vm.messageText && $scope.vm.selectedConversation.ConversationId) {
            cbDataService.postNewMessage($scope.vm.selectedConversation.ConversationId, $scope.vm.messageText).then(
                function successCallback(response) {
                    console.log(response);
                },
                function errorCallback(response) {
                    console.log(response);
                }
            );
        }
        $scope.vm.messageText = '';
    };

    $scope.vm.hub.client.addItem = function (item) {
        console.log('signalR added: ', item);
        $scope.vm.chatLog.push(item);
        $scope.$apply();
    };
    $scope.vm.hub.client.userJoined = function (item) {
        console.log('signalR added: ', item);
        if ($scope.vm.selectedConversation && $scope.vm.selectedConversation.ActiveUsers && item) {
            var len = $scope.vm.selectedConversation.ActiveUsers.length,
            i = 0,
            found = false;
            for (; i < len; i++) {
                if (item.Id === $scope.vm.selectedConversation.ActiveUsers[i].Id) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                $scope.vm.selectedConversation.ActiveUsers.push(item);
                $scope.$apply();
            }
        }
    };
    $scope.vm.hub.client.userLeft = function (item) {
        console.log('signalR removed: ', item);
        if ($scope.vm.selectedConversation && $scope.vm.selectedConversation.ActiveUsers && item) {
            var len = $scope.vm.selectedConversation.ActiveUsers.length,
                i = 0;
            for (; i < len; i++) {
                if (item.Id === $scope.vm.selectedConversation.ActiveUsers[i].Id) {
                    $scope.vm.selectedConversation.ActiveUsers.splice(i, 1);
                    $scope.$apply();
                    break;
                }
            }
        }
    };

    $document.ready(function () {
        $scope.vm.getActiveConversations();
        $.connection.hub.start();
    });
}]);