console.log("PlayView.js loaded");
var DotNet = /** @class */ (function () {
    function DotNet() {
    }
    return DotNet;
}());
var BoardData = /** @class */ (function () {
    function BoardData() {
    }
    return BoardData;
}());
var _canvas;
var _context;
var _size = 10;
var _dragging = 0;
var _mouseDown = false;
var _lastX = 0;
var _lastY = 0;
var _marginLeft = 0;
var _marginTop = 0;
var PIECE_SIZE = 45;
var _boardData;
var _images = [];
var _imagesLoaded = false;
function calculatePosition(event) {
    var column = Math.floor(event.offsetX / PIECE_SIZE);
    var row = Math.floor(event.offsetY / PIECE_SIZE);
    console.log("Clicked canvas at " + column + " - " + row);
    DotNet.invokeMethod("XO.Web", "CanvasClickReceived", column, row);
}
function loadImages() {
    var imagesLoaded = 0;
    var path = "images/";
    var imageFiles = [
        "o_shadowlight.svg", "o_selected.svg", "o_shadowdark.svg", "o.svg", "empty.svg",
        "x.svg", "x_shadowdark.svg", "x_selected.svg", "x_shadowlight.svg"
    ];
    for (var i = 0; i < imageFiles.length; i++) {
        _images[i] = new Image();
        _images[i].onload = function () {
            imagesLoaded++;
            console.log("imagesLoaded: " + imagesLoaded + ", imageFiles.length: " + imageFiles.length);
            if (imagesLoaded === imageFiles.length) {
                _imagesLoaded = true;
                draw(_boardData);
            }
        };
        _images[i].src = path + imageFiles[i];
    }
}
loadImages();
function initializeView(canvas) {
    console.log("PlayView.js initializeView");
    _canvas = canvas;
    _context = _canvas.getContext("2d");
    _context.font = "14pt Arial";
    _canvas.addEventListener('mousedown', function (e) {
        console.log("Mouse down");
        _dragging = 1;
        _mouseDown = true;
        _lastX = e.clientX;
        _lastY = e.clientY;
    }, false);
    _canvas.addEventListener("click", function (event) {
        calculatePosition(event);
    });
    window.addEventListener('resize', function () {
        console.log("PlayView.js resize");
        _canvas.style.marginLeft = "0px";
        _canvas.style.marginTop = "0px";
        _dragging = 0;
        _marginLeft = 0;
        _marginTop = 0;
    }, false);
    window.addEventListener('mousemove', function (e) {
        var calcDeltaX = e.clientX - _lastX;
        var calcDeltaY = e.clientY - _lastY;
        var moveRadius = Math.sqrt(calcDeltaX * calcDeltaX + calcDeltaY * calcDeltaY);
        if (_dragging == 1 && moveRadius > 25) {
            _dragging = 2;
            console.log("Mouse move");
        }
        if (_dragging == 2) {
            var deltaX = e.clientX - _lastX;
            var deltaY = e.clientY - _lastY;
            _lastX = e.clientX;
            _lastY = e.clientY;
            _marginLeft += deltaX;
            _marginTop += deltaY;
            _canvas.style.marginLeft = _marginLeft + "px";
            _canvas.style.marginTop = _marginTop + "px";
        }
        e.preventDefault();
    }, false);
    window.addEventListener('mouseup', function (e) {
        console.log("Mouse up");
        _dragging = 0;
        _mouseDown = false;
    }, false);
}
function draw(boardData) {
    if (_context == null || !_imagesLoaded) {
        return;
    }
    console.log("PlayView.js draw");
    console.log(_boardData);
    _boardData = boardData;
    _context.clearRect(0, 0, _canvas.width, _canvas.height);
    var boardWidth = _size * PIECE_SIZE;
    var boardHeight = _size * PIECE_SIZE;
    for (var i = 0; i <= _size; i++) {
        var lineX = i * PIECE_SIZE;
        var lineY = i * PIECE_SIZE;
        _context.beginPath();
        _context.moveTo(lineX, 0);
        _context.lineTo(lineX, boardHeight);
        _context.stroke();
        _context.beginPath();
        _context.moveTo(0, lineY);
        _context.lineTo(boardWidth, lineY);
        _context.stroke();
    }
    if (_boardData != null) {
        for (var column = 0; column < _size; column++) {
            for (var row = 0; row < _size; row++) {
                var piece = _boardData.data[column][row];
                if (piece != 0) {
                    _context.drawImage(_images[piece + 4], column * PIECE_SIZE + 1, row * PIECE_SIZE + 1);
                }
            }
        }
        if (_boardData.text != null) {
            for (var textLines = 0; textLines < _boardData.text.length; textLines++) {
                var text = _boardData.text[textLines];
                _context.fillText(text, PIECE_SIZE + 1, textLines * PIECE_SIZE + PIECE_SIZE);
            }
        }
    }
}
//# sourceMappingURL=PlayView.js.map