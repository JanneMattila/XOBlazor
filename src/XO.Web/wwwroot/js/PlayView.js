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
var _currentMouseX = 0;
var _currentMouseY = 0;
var _marginLeft = 0;
var _marginTop = 0;
var PIECE_SIZE = 45;
var _boardData;
var _images = [];
var _currentPlayer = "x";
function calculatePosition(event) {
    var column = Math.floor(event.offsetX / PIECE_SIZE);
    var row = Math.floor(event.offsetY / PIECE_SIZE);
    console.log("Clicked canvas at " + column + " - " + row);
    DotNet.invokeMethod("XO.Web", "CanvasClickReceived", column, row);
}
function loadImages() {
    var imagesLoaded = 0;
    var path = "images/";
    var imageFiles = ["x.svg", "x_shadow.svg", "o.svg", "o_shadow.svg"];
    for (var i = 0; i < imageFiles.length; i++) {
        _images[i] = new Image();
        _images[i].onload = function () {
            imagesLoaded++;
            if (imagesLoaded === imageFiles.length) {
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
        //e.preventDefault();
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
        _currentMouseX = -1;
        _currentMouseY = -1;
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
        _currentMouseX = e.clientX;
        _currentMouseY = e.clientY;
        //draw(_boardData);
    }, false);
    window.addEventListener('mouseup', function (e) {
        console.log("Mouse up");
        _dragging = 0;
        _mouseDown = false;
    }, false);
}
function draw(boardData) {
    console.log("PlayView.js draw");
    console.log(_boardData);
    if (_context == null) {
        return;
    }
    _boardData = boardData;
    _context.clearRect(0, 0, _canvas.width, _canvas.height);
    var pieceWidth = PIECE_SIZE;
    var pieceHeight = PIECE_SIZE;
    var boardWidth = _size * pieceWidth;
    var boardHeight = _size * pieceHeight;
    for (var i = 0; i <= _size; i++) {
        var lineX = i * pieceWidth;
        var lineY = i * pieceHeight;
        _context.beginPath();
        _context.moveTo(lineX, 0);
        _context.lineTo(lineX, boardHeight);
        _context.stroke();
        _context.beginPath();
        _context.moveTo(0, lineY);
        _context.lineTo(boardWidth, lineY);
        _context.stroke();
    }
    for (var column = 0; column < _size; column++) {
        for (var row = 0; row < _size; row++) {
            var x = column * pieceWidth;
            var y = row * pieceHeight;
            var index = _size * row + column;
            var piece = String.fromCharCode(_boardData.data[column][row]);
            /*
            if (previousMoveColumn == column && previousMoveRow == row) {
                const fillStyle = _context.fillStyle;
                _context.fillStyle = "#b8ffaa";
                _context.fillRect(x + 1, y + 1, pieceWidth - 2, pieceHeight - 2);
                _context.fillStyle = fillStyle;
            }
            else if (winningMoves != null &&
                winningMoves.indexOf(index) != -1) {
                const fillStyle = _context.fillStyle;
                _context.fillStyle = "#AAAAFF";
                _context.fillRect(x + 1, y + 1, pieceWidth - 2, pieceHeight - 2);
                _context.fillStyle = fillStyle;
            }
            */
            if (piece === 'x') {
                _context.drawImage(_images[0], 9 + x, 8 + y);
            }
            else if (piece === 'o') {
                _context.drawImage(_images[2], 7 + x, 7 + y);
            }
            else if (_dragging == 0 &&
                x >= _currentMouseX - _canvas.offsetLeft - pieceWidth && x < _currentMouseX - _canvas.offsetLeft &&
                y >= _currentMouseY - _canvas.offsetTop - pieceHeight && y < _currentMouseY - _canvas.offsetTop) {
                if (_currentPlayer === "x") {
                    _context.drawImage(_images[1], 7 + x, 7 + y);
                }
                else {
                    _context.drawImage(_images[3], 9 + x, 8 + y);
                }
            }
            /*
            else if (_pendingColumn == column && _pendingRow == row) {

                if (_currentPlayer === "x") {
                    _context.drawImage(_images[1], 7 + x, 7 + y);
                }
                else {
                    _context.drawImage(_images[3], 9 + x, 8 + y);
                }
            }
            */
        }
    }
}
//# sourceMappingURL=PlayView.js.map