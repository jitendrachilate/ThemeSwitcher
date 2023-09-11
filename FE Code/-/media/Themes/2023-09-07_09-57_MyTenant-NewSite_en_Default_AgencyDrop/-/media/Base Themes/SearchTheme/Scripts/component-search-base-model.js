/**
 * include functionality for sorting facet array
 * @module searchBaseModel
 * @param  {jQuery} $ Instance of jQuery
 * @param  {Document} document dom document object
 * @return {baseModel} list of methods for working with facet array
*/
XA.component.search.baseModel = (function ($, document) {
    /**
    * @name module:searchBaseModel.baseModel
    * @constructor
    * @augments Backbone.Model
    */
    return Backbone.Model.extend(
        /** @lends module:searchBaseModel.baseModel.prototype **/
        {
            /**
             * Sort facets by sort order option
             * @param {String} sortOrder method of sorting - SortByCount|SortByNames
             * @param {Array} facetArray  list of facets that should be sorted
             */
            sortFacetArray: function (sortOrder, facetArray) {
                switch (sortOrder) {
                    case 'SortByCount': {
                        facetArray.sort(function (a, b) { return b.Count - a.Count });
                        break;
                    }
                    case 'SortByCountAsc': {
                        facetArray.sort(function (a, b) { return a.Count - b.Count });
                        break;
                    }
                    case 'SortByNamesDesc': {
                        facetArray.sort(function (a, b) {
                            if (a.Name < b.Name) { return 1; }
                            if (a.Name > b.Name) { return -1; }
                            return 0;
                        });
                        break;
                    }
                    case 'SortByNames':
                    default: {
                        facetArray.sort(function (a, b) {
                            if (a.Name < b.Name) { return -1; }
                            if (a.Name > b.Name) { return 1; }
                            return 0;
                        });
                        break;
                    }
                }
            },
            /**
             * Converts facets object into array
             * @param {Object} valuesObject Object with facet values
             * @memberof module:dropdown.FacetDropdownModel
             * @alias module:dropdown.FacetDropdownModel#objectToArray
             */
            objectToArray: function (valuesObject) {
                var valuesArrray = [];
                for (const item in valuesObject) {
                    valuesArrray.push(valuesObject[item]);
                }
                return valuesArrray;
            },
        });

}(jQuery, document));

XA.register('searchBaseModel', XA.component.search.baseModel);
