console.log("PlayView.js loaded");

let _canvasElement: HTMLCanvasElement;
let _context: CanvasRenderingContext2D;
let _size = 20;

window.addEventListener('resize', function () {
    console.log("PlayView.js resize");
}, false);

function initializeView(canvasElement: HTMLCanvasElement) {
    console.log("PlayView.js initializeView");

    _canvasElement = canvasElement;
    _context = _canvasElement.getContext("2d");
    _context.font = "14pt Arial";

    _canvasElement.addEventListener("click", (event: MouseEvent) => {
        //calculatePosition(event);
    });
}

function draw() {
    console.log("PlayView.js draw");

    _context.clearRect(0, 0, _canvasElement.width, _canvasElement.height);

    const pieceWidth = 45;
    const pieceHeight = 45;
    const boardWidth = _size * pieceWidth;
    const boardHeight = _size * pieceHeight;

    for (let i = 0; i <= 10; i++) {
        const lineX = i * pieceWidth;
        const lineY = i * pieceHeight;

        _context.beginPath();
        _context.moveTo(lineX, 0);
        _context.lineTo(lineX, boardHeight);
        _context.stroke();

        _context.beginPath();
        _context.moveTo(0, lineY);
        _context.lineTo(boardWidth, lineY);
        _context.stroke();
    }

    for (let column = 0; column < _size; column++) {
        for (let row = 0; row < _size; row++) {
            const x = column * pieceWidth;
            const y = row * pieceHeight;
            const index = _size * row + column;
        }
    }
}