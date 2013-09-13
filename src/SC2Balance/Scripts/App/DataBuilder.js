var DataBuilder = (function () {

    var dataBuilderConstructor = function () {
    };

    dataBuilderConstructor.prototype = {
        getDataFromRaceWinRate: function (raceWinRate, matchup) {
            return getDataFromRaceWinRate(this, raceWinRate, matchup);
        },
        getDataFromWinRate: function(winRate) {
            return getDataFromWinRate(this, winRate);
        },
        getDataFromBalanceHistory: function (balanceHistory) {
            return getDataFromBalanceHistory(this, balanceHistory);
        }
    };

    function getDataFromBalanceHistory(me, balanceHistory) {
        return balanceHistory;
    }

    function getDataFromRaceWinRate(me, raceWinRate, matchup) {
        var data = [];
        switch (matchup) {
            case 'TVP':
                data.push({
                    race: 'Terran',
                    winRate: raceWinRate.TVPWinRate
                });
                data.push({
                    race: 'Protoss',
                    winRate: 100 - raceWinRate.TVPWinRate
                });
                break;
            case 'TVZ':
                data.push({
                    race: 'Terran',
                    winRate: raceWinRate.TVZWinRate
                });
                data.push({
                    race: 'Zerg',
                    winRate: 100 - raceWinRate.TVZWinRate
                });
                break;
            case 'ZVP':
                data.push({
                    race: 'Zerg',
                    winRate: raceWinRate.ZVPWinRate
                });
                data.push({
                    race: 'Protoss',
                    winRate: 100 - raceWinRate.ZVPWinRate
                });
                break;
            default:
                break;
        }
        return data;
    }

    function getDataFromWinRate(me, winRate) {
        var data = [];
        data.push({
            race: 'Terran',
            winRate: winRate.TerranWinRate
        });
        data.push({
            race: 'Protoss',
            winRate: winRate.ProtossWinRate
        });
        data.push({
            race: 'Zerg',
            winRate: winRate.ZergWinRate
        });
        return data;
    }
    
    return dataBuilderConstructor;
})();