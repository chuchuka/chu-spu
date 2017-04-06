namespace Chuchuka {

	export namespace WebServices {

		export function CallWcfService(pathToService: string, httpMethod: string, wcfType: string, data: any = null, headers: any = null): JQueryXHR {
			if (headers == null) {
				headers = {};
			}

			if (!headers["X-RequestDigest"]) {
				headers["X-RequestDigest"] = $("#__REQUESTDIGEST").val();
			}

			let requestOptions: JQueryAjaxSettings = {
				"url": getWcfServiceUrl(pathToService, httpMethod),
				"type": wcfType,
				"cache": false,
				"data": JSON.stringify(data),
				"contentType": "application/json; charset=utf-8",
				"processData": true
			};
			
			return $.ajax(requestOptions);
		}

		function getWcfServiceUrl(pathToService, method): string {
			return Utilities.Url.CombineUrl(_spPageContextInfo.webAbsoluteUrl, "_vti_bin", pathToService, method);
		}

	}

	export namespace Utilities {

		export namespace Url {

			export function CombineUrl(...arg: string[]): string {
				var result: string = '';
				$.each(arg, (index, value) => {
					result += value.replace(/\/$/g, '') + '/';
				});
				return result.replace(/\/$/g, '');
			}

		}

		export namespace DateTime {

			/**
			 * Get Date from JavaScriptSerializer format
			 * @param date 
			 * @returns {} 
			 */
			export function GetDateFromJssFormat(date: string): Date {
				return new Date(parseInt(date.replace("/Date(", "").replace(")/", ""), 10));
			}

		}

		export namespace FileUploader {

			export function Upload(file: File, name: string, rootFolder: string, metadata: any): JQueryPromise<void> {
				return $.when(getFileBuffer(file))
					.then((buffer) => {
						return addFileToFolder(buffer, name, rootFolder);
					})
					.then((item) => {
						var uri = item.d.ListItemAllFields.__deferred.uri;
						return getListItem(uri);
					})
					.then((item) => {
						let itemData = item.d.__metadata;
						return updateListItem(itemData, metadata);
					});
			}

			function getFileBuffer(file: any): JQueryPromise<any> {
				var deferred = $.Deferred();
				var reader = new FileReader();
				reader.onloadend = () => {
					deferred.resolve(reader.result);
				}
				reader.onerror = (e: any) => {
					deferred.reject(e.target.error);
				}
				reader.readAsArrayBuffer(file);
				return deferred.promise();
			}

			function addFileToFolder(buffer: any, name: string, rootFolder: string): JQueryXHR {
				let parts = name.split('\\');
				let fileName = parts[parts.length - 1];
				let endpoint = String.format("{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files/add(overwrite=true, url='{2}')", _spPageContextInfo.webAbsoluteUrl, rootFolder, fileName);
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

			function getListItem(fileListItemUri): JQueryXHR {
				return $.ajax({
					"url": fileListItemUri,
					"cache": false,
					"headers": {
						"Accept": "application/json; odata=verbose"
					}
				});
			}

			function updateListItem(item, metadata): JQueryXHR {
				let body = {
					"__metadata": {
						"type": item.type
					}
				};
				for (let property in metadata) {
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

		}

	}

}
