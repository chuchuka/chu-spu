var Chuchuka;
(function (Chuchuka) {
    var WebServices;
    (function (WebServices) {
        function CallWcfService(pathToService, httpMethod, wcfType, data, headers) {
            if (data === void 0) { data = null; }
            if (headers === void 0) { headers = null; }
            if (headers == null) {
                headers = {};
            }
            if (!headers["X-RequestDigest"]) {
                headers["X-RequestDigest"] = $("#__REQUESTDIGEST").val();
            }
            var requestOptions = {
                "url": getWcfServiceUrl(pathToService, httpMethod),
                "type": wcfType,
                "cache": false,
                "data": JSON.stringify(data),
                "contentType": "application/json; charset=utf-8",
                "processData": true
            };
            return $.ajax(requestOptions);
        }
        WebServices.CallWcfService = CallWcfService;
        function getWcfServiceUrl(pathToService, method) {
            return Utilities.Url.CombineUrl(_spPageContextInfo.webAbsoluteUrl, "_vti_bin", pathToService, method);
        }
    })(WebServices = Chuchuka.WebServices || (Chuchuka.WebServices = {}));
    var Utilities;
    (function (Utilities) {
        var Url;
        (function (Url) {
            function CombineUrl() {
                var arg = [];
                for (var _i = 0; _i < arguments.length; _i++) {
                    arg[_i - 0] = arguments[_i];
                }
                var result = '';
                $.each(arg, function (index, value) {
                    result += value.replace(/\/$/g, '') + '/';
                });
                return result.replace(/\/$/g, '');
            }
            Url.CombineUrl = CombineUrl;
        })(Url = Utilities.Url || (Utilities.Url = {}));
        var DateTime;
        (function (DateTime) {
            /**
             * Get Date from JavaScriptSerializer format
             * @param date
             * @returns {}
             */
            function GetDateFromJssFormat(date) {
                return new Date(parseInt(date.replace("/Date(", "").replace(")/", ""), 10));
            }
            DateTime.GetDateFromJssFormat = GetDateFromJssFormat;
        })(DateTime = Utilities.DateTime || (Utilities.DateTime = {}));
        var FileUploader;
        (function (FileUploader) {
            function Upload(file, name, rootFolder, metadata) {
                return $.when(getFileBuffer(file))
                    .then(function (buffer) {
                    return addFileToFolder(buffer, name, rootFolder);
                })
                    .then(function (item) {
                    var uri = item.d.ListItemAllFields.__deferred.uri;
                    return getListItem(uri);
                })
                    .then(function (item) {
                    var itemData = item.d.__metadata;
                    return updateListItem(itemData, metadata);
                });
            }
            FileUploader.Upload = Upload;
            function getFileBuffer(file) {
                var deferred = $.Deferred();
                var reader = new FileReader();
                reader.onloadend = function () {
                    deferred.resolve(reader.result);
                };
                reader.onerror = function (e) {
                    deferred.reject(e.target.error);
                };
                reader.readAsArrayBuffer(file);
                return deferred.promise();
            }
            function addFileToFolder(buffer, name, rootFolder) {
                var parts = name.split('\\');
                var fileName = parts[parts.length - 1];
                var endpoint = String.format("{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files/add(overwrite=true, url='{2}')", _spPageContextInfo.webAbsoluteUrl, rootFolder, fileName);
                return $.ajax({
                    "url": endpoint,
                    "type": "POST",
                    "data": buffer,
                    "processData": false,
                    "cache": false,
                    "headers": {
                        "Accept": "application/json; odata=verbose",
                        "X-RequestDigest": $("#__REQUESTDIGEST").val()
                    }
                });
            }
            function getListItem(fileListItemUri) {
                return $.ajax({
                    "url": fileListItemUri,
                    "cache": false,
                    "headers": {
                        "Accept": "application/json; odata=verbose"
                    }
                });
            }
            function updateListItem(item, metadata) {
                var body = {
                    "__metadata": {
                        "type": item.type
                    }
                };
                for (var property in metadata) {
                    if (metadata.hasOwnProperty(property)) {
                        body[property] = metadata[property];
                    }
                }
                return $.ajax({
                    "url": item.uri,
                    "type": "POST",
                    "cache": false,
                    "data": JSON.stringify(body),
                    "headers": {
                        "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                        "Content-Type": "application/json; odata=verbose",
                        "If-Match": item.etag,
                        "X-HTTP-Method": "MERGE"
                    }
                });
            }
        })(FileUploader = Utilities.FileUploader || (Utilities.FileUploader = {}));
    })(Utilities = Chuchuka.Utilities || (Chuchuka.Utilities = {}));
})(Chuchuka || (Chuchuka = {}));
//# sourceMappingURL=Chuchuka.js.map