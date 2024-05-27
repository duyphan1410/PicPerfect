const fileInput = document.querySelector(".file-input"),
    filterOptions = document.querySelectorAll(".filter button"),
    filterName = document.querySelector(".filter-info .name"),
    filterValue = document.querySelector(".filter-info .value"),
    filterSlider = document.querySelector(".slider input"),
    rotateOptions = document.querySelectorAll(".rotate button"),
    previewImg = document.querySelector(".preview-img img"),
    resetFilterBtn = document.querySelector(".reset-filter"),
    chooseImgBtn = document.querySelector(".choose-img"),
    saveImgBtn = document.querySelector(".save-img");

let brightness = "100", saturation = "100", inversion = "0", grayscale = "0";
let rotate = 0, flipHorizontal = 1, flipVertical = 1;

const loadImage = () => {
    let file = fileInput.files[0];
    if (!file) return;
    previewImg.src = URL.createObjectURL(file);
    previewImg.addEventListener("load", () => {
        resetFilterBtn.click();
        document.querySelector(".container").classList.remove("disable");
    });
}

const applyFilter = () => {
    previewImg.style.transform = `rotate(${rotate}deg) scale(${flipHorizontal}, ${flipVertical})`;
    previewImg.style.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
}

filterOptions.forEach(option => {
    option.addEventListener("click", () => {
        document.querySelector(".active").classList.remove("active");
        option.classList.add("active");
        filterName.innerText = option.innerText;

        if (option.id === "brightness") {
            filterSlider.max = "200";
            filterSlider.value = brightness;
            filterValue.innerText = `${brightness}%`;
        } else if (option.id === "saturation") {
            filterSlider.max = "200";
            filterSlider.value = saturation;
            filterValue.innerText = `${saturation}%`
        } else if (option.id === "inversion") {
            filterSlider.max = "100";
            filterSlider.value = inversion;
            filterValue.innerText = `${inversion}%`;
        } else {
            filterSlider.max = "100";
            filterSlider.value = grayscale;
            filterValue.innerText = `${grayscale}%`;
        }
    });
});

const updateFilter = () => {
    filterValue.innerText = `${filterSlider.value}%`;
    const selectedFilter = document.querySelector(".filter .active");

    if (selectedFilter.id === "brightness") {
        brightness = filterSlider.value;
    } else if (selectedFilter.id === "saturation") {
        saturation = filterSlider.value;
    } else if (selectedFilter.id === "inversion") {
        inversion = filterSlider.value;
    } else {
        grayscale = filterSlider.value;
    }
    applyFilter();
}

rotateOptions.forEach(option => {
    option.addEventListener("click", () => {
        if (option.id === "left") {
            rotate -= 90;
        } else if (option.id === "right") {
            rotate += 90;
        } else if (option.id === "horizontal") {
            flipHorizontal = flipHorizontal === 1 ? -1 : 1;
        } else {
            flipVertical = flipVertical === 1 ? -1 : 1;
        }
        applyFilter();
    });
});

const resetFilter = () => {
    brightness = "100"; saturation = "100"; inversion = "0"; grayscale = "0";
    rotate = 0; flipHorizontal = 1; flipVertical = 1;
    filterOptions[0].click();
    applyFilter();
}

document.querySelector('.save-img').addEventListener('click', async () => {
    const username = '@HttpContext.Session.GetString("username")'; // Lấy giá trị của session username
    if (username) {
        // Người dùng đã đăng nhập, cho phép họ lưu hình ảnh
        await saveImage();
    } else {
        // Người dùng chưa đăng nhập, chuyển hướng họ đến trang đăng nhập
        window.location.href = '/Login';
    }
});

const saveImage = async () => {
    try {
        const canvas = document.createElement("canvas");
        const ctx = canvas.getContext("2d");
        canvas.width = previewImg.naturalWidth;
        canvas.height = previewImg.naturalHeight;

        // Áp dụng các bộ lọc và biến đổi vào hình ảnh
        ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
        ctx.translate(canvas.width / 2, canvas.height / 2);
        if (rotate !== 0) {
            ctx.rotate(rotate * Math.PI / 180);
        }
        ctx.scale(flipHorizontal, flipVertical);
        ctx.drawImage(previewImg, -canvas.width / 2, -canvas.height / 2, canvas.width, canvas.height);

        // Tạo một đối tượng blob từ hình ảnh trên canvas
        const blob = await new Promise(resolve => canvas.toBlob(resolve));
        const formData = new FormData();
        formData.append("file", blob, "image.jpg");

        // Gửi yêu cầu POST lên endpoint /SaveImage
        const response = await fetch("/SaveImage", {
            method: "POST",
            body: formData
        });

        // Tạo một liên kết để tải xuống hình ảnh đã chỉnh sửa
        const link = document.createElement("a");
        link.download = "image.jpg";
        link.href = canvas.toDataURL();
        link.click();

        // Xử lý phản hồi từ máy chủ
        if (response.ok) {
            const message = await response.text();
            console.log("Server response:", message);
        } else {
            console.error("Failed to save image:", response.statusText);
        }
    } catch (error) {
        console.error("Error saving image:", error);
    }
}

filterSlider.addEventListener("input", updateFilter);
resetFilterBtn.addEventListener("click", resetFilter);
saveImgBtn.addEventListener("click", saveImage);
fileInput.addEventListener("change", loadImage);
chooseImgBtn.addEventListener("click", () => fileInput.click());

//Drawing

//const canvas = document.querySelector("canvas"),
//toolBtns = document.querySelectorAll(".tool"),
//fillColor = document.querySelector("#fill-color"),
//sizeSlider = document.querySelector("#size-slider"),
//colorBtns = document.querySelectorAll(".colors .option"),
//colorPicker = document.querySelector("#color-picker"),
//clearCanvas = document.querySelector(".clear-canvas"),
//ctx = canvas.getContext("2d");

//// global variables with default value
//let prevMouseX, prevMouseY, snapshot,
//isDrawing = false,
//selectedTool = "brush",
//brushWidth = 5,
//selectedColor = "#000";

//const setCanvasBackground = () => {
//    // setting whole canvas background to white, so the downloaded img background will be white
//    ctx.fillStyle = "#fff";
//    ctx.fillRect(0, 0, canvas.width, canvas.height);
//    ctx.fillStyle = selectedColor; // setting fillstyle back to the selectedColor, it'll be the brush color
//}

//window.addEventListener("load", () => {
//    // setting canvas width/height.. offsetwidth/height returns viewable width/height of an element
//    canvas.width = canvas.offsetWidth;
//    canvas.height = canvas.offsetHeight;
//    setCanvasBackground();
//});



//const startDraw = (e) => {
//    isDrawing = true;
//    prevMouseX = e.offsetX; // passing current mouseX position as prevMouseX value
//    prevMouseY = e.offsetY; // passing current mouseY position as prevMouseY value
//    ctx.beginPath(); // creating new path to draw
//    ctx.lineWidth = brushWidth; // passing brushSize as line width
//    ctx.strokeStyle = selectedColor; // passing selectedColor as stroke style
//    ctx.fillStyle = selectedColor; // passing selectedColor as fill style
//    // copying canvas data & passing as snapshot value.. this avoids dragging the image
//    snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
//}

//const drawing = (e) => {
//    if(!isDrawing) return; // if isDrawing is false return from here
//    ctx.putImageData(snapshot, 0, 0); // adding copied canvas data on to this canvas

//    if(selectedTool === "brush" || selectedTool === "eraser") {
//        ctx.strokeStyle = selectedTool === "eraser" ? "#fff" : selectedColor;
//        ctx.lineTo(e.offsetX, e.offsetY); // creating line according to the mouse pointer
//        ctx.stroke(); // drawing/filling line with color
//    } 
//}

//toolBtns.forEach(btn => {
//    btn.addEventListener("click", () => { // adding click event to all tool option
//        // removing active class from the previous option and adding on current clicked option
//        document.querySelector(".options .active").classList.remove("active");
//        btn.classList.add("active");
//        selectedTool = btn.id;
//    });
//});

//sizeSlider.addEventListener("change", () => brushWidth = sizeSlider.value); 

//colorBtns.forEach(btn => {
//    btn.addEventListener("click", () => { 
//        document.querySelector(".options .selected").classList.remove("selected");
//        btn.classList.add("selected");
//        selectedColor = window.getComputedStyle(btn).getPropertyValue("background-color");
//    });
//});

//colorPicker.addEventListener("change", () => {
//    colorPicker.parentElement.style.background = colorPicker.value;
//    colorPicker.parentElement.click();
//});

//clearCanvas.addEventListener("click", () => {
//    ctx.clearRect(0, 0, canvas.width, canvas.height); // clearing whole canvas
//    setCanvasBackground();
//});

//canvas.addEventListener("mousedown", startDraw);
//canvas.addEventListener("mousemove", drawing);
//canvas.addEventListener("mouseup", () => isDrawing = false);