console.log("PlayView.js loaded");

class DotNet {
    static invokeMethod: Function;
}

class BoardData {
    moves: Array<number>;
    data: Int8Array[];
}

let _canvas: HTMLCanvasElement;
let _context: CanvasRenderingContext2D;

let _size = 10;
let _dragging = 0;
let _mouseDown = false;
let _lastX = 0;
let _lastY = 0;
let _marginLeft = 0;
let _marginTop = 0;

const PIECE_SIZE = 45;

let _boardData: BoardData;
let _images: HTMLImageElement[] = [];
let _imagesLoaded = false;

function calculatePosition(event: MouseEvent): void {
    let column = Math.floor(event.offsetX / PIECE_SIZE);
    let row = Math.floor(event.offsetY / PIECE_SIZE);

    console.log("Clicked canvas at " + column + " - " + row);

    DotNet.invokeMethod("XO.Web", "CanvasClickReceived", column, row);
}

function loadImages() {
    let imagesLoaded = 0;
    const path = "images/";
    const imageFiles = [
        "o_shadowlight.svg", "o_selected.svg", "o_shadowdark.svg", "o.svg", "empty.svg",
        "x.svg", "x_shadowdark.svg", "x_selected.svg", "x_shadowlight.svg"];
    for (let i = 0; i < imageFiles.length; i++) {
        _images[i] = new Image();
        _images[i].onload = function () {
            imagesLoaded++;

            console.log(`imagesLoaded: ${imagesLoaded}, imageFiles.length: ${imageFiles.length}`);
            if (imagesLoaded === imageFiles.length) {
                _imagesLoaded = true;
                draw(_boardData);
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
    }, false);

    _canvas.addEventListener("click", (event: MouseEvent) => {
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
    }, false);

    window.addEventListener('mouseup', function (e) {
        console.log("Mouse up");

        _dragging = 0;
        _mouseDown = false;
    }, false);
}

function draw(boardData: BoardData) {
    if (_context == null || !_imagesLoaded) {
        return;
    }

    console.log("PlayView.js draw");
    console.log(_boardData);

    _boardData = boardData;
    _context.clearRect(0, 0, _canvas.width, _canvas.height);

    const boardWidth = _size * PIECE_SIZE;
    const boardHeight = _size * PIECE_SIZE;

    for (let i = 0; i <= _size; i++) {
        const lineX = i * PIECE_SIZE;
        const lineY = i * PIECE_SIZE;

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
        for (let column = 0; column < _size; column++) {
            for (let row = 0; row < _size; row++) {
                const piece = _boardData.data[column][row];
                if (piece != 0) {
                    _context.drawImage(_images[piece + 4], column * PIECE_SIZE + 1, row * PIECE_SIZE + 1);
                }
            }
        }
    }
}
