console.log("PlayView.js loaded");

let _canvas: HTMLCanvasElement;
let _context: CanvasRenderingContext2D;

let _size = 20;
let _dragging = 0;
let _mouseDown = false;
let _lastX = 0;
let _lastY = 0;
let _currentMouseX = 0;
let _currentMouseY = 0;
let _marginLeft = 0;
let _marginTop = 0;

let _images: HTMLImageElement[] = [];
let _board: Int8Array[] = [];
let _currentPlayer = "x";

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

    const calcDeltaX = e.clientX - _lastX;
    const calcDeltaY = e.clientY - _lastY;
    const moveRadius = Math.sqrt(calcDeltaX * calcDeltaX + calcDeltaY * calcDeltaY);

    if (_dragging == 1 && moveRadius > 25) {
        _dragging = 2;
        console.log("Mouse move");
    }

    if (_dragging == 2) {
        const deltaX = e.clientX - _lastX;
        const deltaY = e.clientY - _lastY;
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

function loadImages() {
    let imagesLoaded = 0;
    const path = "images/";
    const imageFiles = ["x.svg", "x_shadow.svg", "o.svg", "o_shadow.svg"];
    for (let i = 0; i < imageFiles.length; i++) {
        _images[i] = new Image();
        _images[i].onload = function () {
            imagesLoaded++;

            if (imagesLoaded === imageFiles.length) {
                draw();
            }
        };
        _images[i].src = path + imageFiles[i];
    }
}

loadImages();

function initializeView(canvas: HTMLCanvasElement) {
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

    _canvas.addEventListener("click", (event: MouseEvent) => {
        //calculatePosition(event);
    });
}

function draw() {
    console.log("PlayView.js draw");
    if (_context == null) {
        return;
    }

    _context.clearRect(0, 0, _canvas.width, _canvas.height);

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

            if (_dragging == 0 &&
                x >= _currentMouseX - _canvas.offsetLeft - pieceWidth && x < _currentMouseX - _canvas.offsetLeft &&
                y >= _currentMouseY - _canvas.offsetTop - pieceHeight && y < _currentMouseY - _canvas.offsetTop) {

                if (_currentPlayer === "x") {
                    _context.drawImage(_images[1], 7 + x, 7 + y);
                }
                else {
                    _context.drawImage(_images[3], 9 + x, 8 + y);
                }
            }
        }
    }
}