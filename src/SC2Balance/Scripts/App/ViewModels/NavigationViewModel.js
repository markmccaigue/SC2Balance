var NavigationViewModel = (function () {

    var navigationViewModelConstructor = function () {
        var balanceName = 'Balance';
        this.currentPage = ko.observable(balanceName);
        this.loadedPages = [balanceName];
    };

    navigationViewModelConstructor.prototype = {
        pageSelected: function (newPage) {
            return pageSelected(this, newPage);
        },
        isCurrentPage: function (page) {
            return isCurrentPage(this, page);
        }
    };

    function pageSelected(me, newPage) {
        if (newPage === me.currentPage()) {
            return;
        }
        if ($.inArray(newPage, me.loadedPages) == -1) {
            loadPage(me, newPage);
            me.loadedPages.push(newPage);
            return;
        }
        me.currentPage(newPage);
    }
    
    function isCurrentPage(me, page) {
        return page === me.currentPage();
    }

    function loadPage(me, page) {
        LoadingManager().startLoad();
        me.currentPage(page);
        switch (page) {
            case 'Maps':
                new MapsRunner().loadMaps();
                break;
            case 'History':
                new HistoryRunner().loadHistory();
                break;
            default:
                break;
        }
        LoadingManager().endLoad();
    }

    return navigationViewModelConstructor;
})();