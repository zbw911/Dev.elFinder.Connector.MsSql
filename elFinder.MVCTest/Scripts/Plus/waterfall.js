function waterfall(){return this.init.apply(this,arguments)}
waterfall.prototype = {
    init:function(opts){
        var set = this.extend({
            boxID:'box',//最外层
            contentID:'content',//内容层
            margin:8,//每块内容之间的间距
            minColumn:4,//最小列数
            url:'data.html'//加载新数据的地址
        },opts||{});
        var _this = this;
        this.set = set;
        this.box = this.$(set.boxID);
        this.boxHeight = this.box.offsetHeight;
        this.content = this.$(set.contentID);
        this.isLoading = false;
        this.timer = null;
        this.loadTime = 1;//这个是用来做测试的，也是作为ulr后面的查询语句，用在实际项目中根据实际需求改
        this.setElementsPos();
        this.box.style.display = "block";
        window.onresize = function(){_this.resize()}
    },
    /*设置每块内容的位置，以及整体内容的最大高度*/
    setElementsPos:function(){
        var set = this.set,pos = [],position = [],i=0,len,items,itemWidth,column,n,m;
        items = this.$$('div', 'poster_wall', this.content);
        if (items.length > 0) {
            itemWidth = items[0].offsetWidth;
            len = items.length;
            this.box.style.width = (document.body.clientWidth > 1008 ? 1008 : 1008) + "px";
            column = Math.max(Math.floor(this.box.offsetWidth / (set.margin + itemWidth)), set.minColumn || 0);
            m = Math.floor(len / column);
            for (; i < len; i++) {
                n = i % column;
                position[n] !== undefined ? (position[n] += items[i - column].offsetHeight + set.margin) : (position[n] = 0);
                items[i].style.top = position[n] + 'px';
                items[i].style.left = n * (items[i].offsetWidth + set.margin) + 'px';
                if (i > (n === 0 ? m - 1 : m) * column - 1) pos.push(items[i].offsetHeight + position[n])
            };
            this.content.style.height = Math.max.apply(null, pos) + 'px';
            this.content.style.width = column * (itemWidth + set.margin) + 'px';
        }
        this.doScroll();
    },
    /*拖动滚动条*/
    doScroll:function(){
        var box = this.box,boxHeight = this.boxHeight,content = this.content,contentHeight= this.content.offsetHeight;
        var _this = this;


        window.onscroll = function () {
            if (($(window).height() + $(window).scrollTop()) + 250 >= $("body").height()) {
                _this.load();
            }
        }
        
        box.onscroll = function(){
            var st = box.scrollTop;
            if(st > contentHeight - boxHeight - 10||1==1){
                _this.load();
            }
        }


        //        var nScrollHight = 0; //滚动距离总长(注意不是滚动条的长度)
        //var nScrollTop = 0;   //滚动到的当前位置
        //var nDivHight = box.offsetHeight;

        //box.onscroll(function () {
        //    nScrollHight = box.scrollHeight;
        //    nScrollTop = box.scrollTop;
        //  if(nScrollTop + nDivHight >= nScrollHight)
        //    alert("滚动条到底部了");
        //  });





    },
    /*改变外框大小*/
    resize:function(){
        var _this = this,set = this.set;
        if(this.timer) clearTimeout(this.timer);
        this.timer = setTimeout(function(){
            _this.setElementsPos();
        },200);
    },
    /*当滚动条拉倒最下面的时候，ajax加载内容*/
    load:function(){
        if(this.loadTime == 5 || this.isLoading) return;
        this.isLoading = true;
        var xhr = this.createXhr(),_this = this,set = this.set;
        xhr.open('GET',set.url+'?&page='+this.loadTime);
        xhr.onreadystatechange = function(){
            if(xhr.readyState == 4){
                if(xhr.status == 200){
                    ++_this.loadTime;
                    _this.isLoading = false;
                    _this.insertData(xhr.responseText);
                }
            }
        }
        xhr.send(null);
    },
    /*将load下来的数据插入到content中，并重新计算和设置每个元素的位置*/
    insertData:function(data){
        var frag = document.createDocumentFragment(),div = document.createElement('div'),els;
        div.innerHTML = data;
        els = this.$$('div', 'poster_wall', div);
        for(var i=0,len=els.length;i<len;i++){
            frag.appendChild(els[i].cloneNode(true));
        };
        this.content.appendChild(frag);
        this.setElementsPos();
        
    },
    createXhr:function(){
        if(typeof XMLHttpRequest != 'undefined'){
            return new XMLHttpRequest();
        }else{
            var xhr = null;
            try{
                xhr = new ActiveXObject('MSXML2.XmlHttp.6.0');
                return xhr;
            }catch(e){
                try{
                    xhr  = new ActiveXObject('MSXML2.XmlHttp.3.0');
                    return xhr;
                }catch(e){
                    throw Error('cannot create XMLHttp object!');
                };
            };
        };
    },
    $:function(id){
        return typeof id == 'string' ? document.getElementById(id) : id;
    },
    $$:function(node,oClass,parent){
        var re = [],els,el,i=0;
        parent = parent || document;
        els = parent.getElementsByTagName(node);
        for(;i<els.length;i++){
            el = els[i];
            if((' '+el.className+' ').indexOf(' '+oClass+' ') > -1) re.push(el);
        };
        return re;
    },
    extend:function(target,o){
        for (var key in o) target[key] = o[key];
        return target;
    }
}// JavaScript Document