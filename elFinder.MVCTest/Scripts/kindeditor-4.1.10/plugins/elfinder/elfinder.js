
KindEditor.plugin('elfinder', function (K) {

    var self = this, name = 'elfinder',
		fileManagerJson = K.undef(self.fileManagerJson, self.basePath + 'php/file_manager_json.php'),
		imgPath = self.pluginsPath + name + '/images/',
		lang = self.lang(name + '.');
    function makeFileTitle(filename, filesize, datetime) {
        return filename + ' (' + Math.ceil(filesize / 1024) + 'KB, ' + datetime + ')';
    }
    function bindTitle(el, data) {
        if (data.is_dir) {
            el.attr('title', data.filename);
        } else {
            el.attr('title', makeFileTitle(data.filename, data.filesize, data.datetime));
        }
    }
    self.plugin.filemanagerDialog = function (options) {
        var clickFn = options.clickFn;

        OpenElFinderDialog(function (hash) {
            urlFromHash(hash, function (url) {
                clickFn(url, hash);
            });
        });
    };
});
