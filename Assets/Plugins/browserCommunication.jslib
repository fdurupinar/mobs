mergeInto(LibraryManager.library, {

 
    SendDemographicsToPage: function(gender, age, nationality, familiarity){
          receiveDemographicsFromUnity(UTF8ToString(gender), age, UTF8ToString(nationality), familiarity);
    },

    SendSurveyToPage: function(type, quality, responses, size, crowdPersonality){

        var arr=new Array();
        for(var i=0;i<size;i++){
            arr[i] = HEAPF32[(responses>>2)+i];
            
        }

          receiveSurveyFromUnity(UTF8ToString(type), quality, arr, crowdPersonality);
    },

    
    SendUserStatsToPage: function( timeSpent,  fightCnt, punchCnt,  avgSpeed,  totalDist,  collectedItemCnt,  stolenItemCnt, finalItemCnt, crowdPersonality){
        receiveUserStatsFromUnity(timeSpent,  fightCnt,  punchCnt, avgSpeed,  totalDist,  collectedItemCnt,  stolenItemCnt, finalItemCnt, crowdPersonality);
    },


    SendMissionMessageToPage: function(message){
        receiveMissionMessageFromUnity(UTF8ToString(message));
    },


    SendCompletedToPage: function(){
        receiveCompletedFromUnity();
    }

});