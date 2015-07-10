function OpenElFinderDialog(callback) {
    var fm = $('<div/>').dialogelfinder({
        url: '/elfinder.connector',
        lang: 'zh_CN',
        width: 840,
        destroyOnClose: true,
        getFileCallback: function (files, fm) {

        },
        handlers: {
            dblclick: function (event, elfinderInstance) {
                //debugger;

                var data = event.data.file;

                var hash = Decode(data);

                hash = bin2String(hash);

                if (hash.indexOf(".") > 0) {

                    if ($.isFunction(callback)) {

                        callback(hash);
                    }

                    $('.dialogelfinder-drag-close').click();
                } else {
                    //window.alert('folder');
                }
                ////elfinderInstance.dialogelfinder('close');


                //console.log(event.data);
                //console.log(event.data.file); // selected files hashes list
            }
        }

    }).dialogelfinder('instance');
}


function bin2String(unicode) {
    var str = '';
    for (var i = 0, len = unicode.length ; i < len ; ++i) {
        str += String.fromCharCode(unicode[i]);
    }
    return str;
}

function Encode(input) {
    //	byte[] bytes = Encoding.UTF8.GetBytes( input );
    //string encoded = Convert.ToBase64String( bytes, Base64FormattingOptions.None );
    //// need to replace some special chars to make whole string compatible
    //encoded = encoded.Replace( '+', '«' )
    //                .Replace( '/', '»' )
    //                .Replace( '=', '§' );
    //return encoded;
}

function Decode(hash) {

    hash = hash.substr(3);
    hash = hash.replace(/«/g, '+')
                    .replace(/»/g, '/')
                    .replace(/§/g, '=');


    return BASE64.decoder(hash);
}


function urlFromHash(hash, callback) {
    hash = $.trim(hash);

    $.get("/elfinder.ParseFileKey", {
        key: hash,
        r: $.now
    }, function (url) {
        callback(url);
    });

}


function testhash(hash) {

    var ext = hash.substr(hash.indexOf('.') + 1).toLowerCase();//!= "flv"

    //ext = 'zip';
    for (var i in ext) {
        window.alert(ext[i].charCodeAt());
    }
}