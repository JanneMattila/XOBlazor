console.log("PlayView.js loaded");
var _canvas;
var _context;
var _size = 20;
var _dragging = 0;
var _mouseDown = false;
var _lastX = 0;
var _lastY = 0;
var _currentMouseX = 0;
var _currentMouseY = 0;
var _marginLeft = 0;
var _marginTop = 0;
window.addEventListener('resize', function () {
    console.log("PlayView.js resize");
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
    draw();
}, false);
window.addEventListener('mouseup', function (e) {
    console.log("Mouse up");
    _dragging = 0;
    _mouseDown = false;
}, false);
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
        e.preventDefault();
    }, false);
    _canvas.addEventListener("click", function (event) {
        //calculatePosition(event);
    });
}
function draw() {
    console.log("PlayView.js draw");
    _context.clearRect(0, 0, _canvas.width, _canvas.height);
    var pieceWidth = 45;
    var pieceHeight = 45;
    var boardWidth = _size * pieceWidth;
    var boardHeight = _size * pieceHeight;
    for (var i = 0; i <= 10; i++) {
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
        }
    }
}
//# sourceMappingURL=PlayView.js.map