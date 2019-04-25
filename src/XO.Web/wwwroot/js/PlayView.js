console.log("PlayView.js loaded");
var _canvasElement;
var _context;
var _size = 20;
window.addEventListener('resize', function () {
    console.log("PlayView.js resize");
}, false);
function initializeView(canvasElement) {
    console.log("PlayView.js initializeView");
    _canvasElement = canvasElement;
    _context = _canvasElement.getContext("2d");
    _context.font = "14pt Arial";
    _canvasElement.addEventListener("click", function (event) {
        //calculatePosition(event);
    });
}
function draw() {
    console.log("PlayView.js draw");
    _context.clearRect(0, 0, _canvasElement.width, _canvasElement.height);
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