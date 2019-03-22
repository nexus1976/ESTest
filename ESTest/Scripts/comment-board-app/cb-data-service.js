'use strict';
angular.module('cb').service('cb-data-service', ['$http', '$httpParamSerializerJQLike', function ($http, $httpParamSerializerJQLike) {
    $http.defaults.headers.common.Authorization = 'Bearer ' + oathToken;
    var urlBase = apiURL + 'api';

    this.getActiveConversations = function () {
        var config = {
            method: 'GET',
            url: urlBase + '/conversations'
        };
        return $http(config);
    };
    this.getConversationDetails = function (conversationId) {
        var config = {
            method: 'GET',
            url: urlBase + '/conversations/' + conversationId
        };
        return $http(config);
    };
    this.getLastSeveralMessages = function (conversationId) {
        var config = {
            method: 'GET',
            url: urlBase + '/conversations/' + conversationId + '/messages'
        };
        return $http(config);
    };
    this.postNewMessage = function (conversationId, message) {
        var messageItem = {
            MessageId: 0,
            CreatedByUserId: 0,
            CreatedByDisplayName: '',
            MessageText: message,
            ConversationId: conversationId,
            CreatedDateTime: new Date()
        };
        var config = {
            method: 'POST',
            url: urlBase + '/conversations/' + conversationId + '/messages',
            data: messageItem
        };
        return $http(config);
    };
}]);