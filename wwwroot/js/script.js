const fileInput = document.querySelector(".file-input"),
  filterOptions = document.querySelectorAll(".filter button"),
  filterName = document.querySelector(".filter-info .name"),
  filterValue = document.querySelector(".filter-info .value"),
  filterSlider = document.querySelector(".slider input"),
  rotateOptions = document.querySelectorAll(".rotate button"),
  previewImg = document.querySelector(".preview-img img"),
  resetFilterBtn = document.querySelector(".reset-filter"),
  chooseImgBtn = document.querySelector(".choose-img"),
  saveImgBtn = document.querySelector(".save-img"),
  localImageInput = document.getElementById("localImageInput"),
  savedImages = document.getElementById("savedImages");

let brightness = 100,
  saturation = 100,
  inversion = 0,
  grayscale = 0;
let rotate = 0,
  flipHorizontal = 1,
  flipVertical = 1;

// Biến để lưu trữ ID ảnh hiện tại
let currentImageId = null;

const loadImage = () => {
  let file = fileInput.files[0];
  if (!file) return;
  previewImg.src = URL.createObjectURL(file);
  previewImg.addEventListener("load", () => {
    resetFilterBtn.click();
    document.querySelector(".container").classList.remove("disable");
  });
};

const applyFilter = () => {
  previewImg.style.transform = `rotate(${rotate}deg) scale(${flipHorizontal}, ${flipVertical})`;
  previewImg.style.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
};

filterOptions.forEach((option) => {
  option.addEventListener("click", () => {
    document.querySelector(".filter .active").classList.remove("active");
    option.classList.add("active");
    filterName.innerText = option.innerText;

    if (option.id === "brightness") {
      filterSlider.max = "200";
      filterSlider.value = brightness;
      filterValue.innerText = `${brightness}%`;
    } else if (option.id === "saturation") {
      filterSlider.max = "200";
      filterSlider.value = saturation;
      filterValue.innerText = `${saturation}%`;
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
};

rotateOptions.forEach((option) => {
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
  brightness = 100;
  saturation = 100;
  inversion = 0;
  grayscale = 0;
  rotate = 0;
  flipHorizontal = 1;
  flipVertical = 1;
  filterOptions[0].click();
  applyFilter();
};

const loadSavedImages = async () => {
  try {
    const response = await fetch("/Home/GetUserImages");
    const result = await response.json();

    if (result.success) {
      savedImages.innerHTML = result.images
        .map(
          (image) => `
        <div class="col-md-6 mb-3">
          <div class="card">
            <img src="${image.imagePath}" class="card-img-top" alt="${image.imageName}" style="height: 150px; object-fit: cover;">
            <div class="card-body">
              <p class="card-text small">${image.imageName}</p>
              <button class="btn btn-sm btn-primary select-saved-image" 
                data-url="${image.imagePath}"
                data-id="${image.imageId}">
                Chọn ảnh này
              </button>
            </div>
          </div>
        </div>
      `
        )
        .join("");

      document.querySelectorAll(".select-saved-image").forEach((btn) => {
        btn.addEventListener("click", () => {
          const imageUrl = btn.dataset.url;
          currentImageId = btn.dataset.id; // Lưu ID ảnh
          previewImg.crossOrigin = "anonymous";
          previewImg.src = imageUrl;
          previewImg.onload = () => {
            resetFilterBtn.click();
            document.querySelector(".container").classList.remove("disable");
            bootstrap.Modal.getInstance(
              document.getElementById("imageModal")
            ).hide();
          };
        });
      });
    }
  } catch (error) {
    console.error("Error loading saved images:", error);
  }
};

const handleLocalImageSelect = (file) => {
  if (!file) return;
  previewImg.crossOrigin = "anonymous";
  previewImg.src = URL.createObjectURL(file);
  previewImg.onload = () => {
    resetFilterBtn.click();
    document.querySelector(".container").classList.remove("disable");
    bootstrap.Modal.getInstance(document.getElementById("imageModal")).hide();
  };
};

chooseImgBtn.addEventListener("click", () => {
  loadSavedImages();
  const modal = new bootstrap.Modal(document.getElementById("imageModal"));
  modal.show();
});

localImageInput.addEventListener("change", (e) => {
  const file = e.target.files[0];
  if (file) {
    handleLocalImageSelect(file);
  }
});

const saveImage = async () => {
  // Kiểm tra xem đã có ảnh được chọn chưa
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    alert("Vui lòng chọn ảnh trước khi lưu!");
    return;
  }

  // Hiển thị overlay loading
  const loadingOverlay = document.querySelector(".loading-overlay");
  loadingOverlay.style.display = "flex";

  try {
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    canvas.width = previewImg.naturalWidth;
    canvas.height = previewImg.naturalHeight;

    ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
    ctx.translate(canvas.width / 2, canvas.height / 2);
    if (rotate !== 0) {
      ctx.rotate((rotate * Math.PI) / 180);
    }
    ctx.scale(flipHorizontal, flipVertical);
    ctx.drawImage(
      previewImg,
      -canvas.width / 2,
      -canvas.height / 2,
      canvas.width,
      canvas.height
    );

    const blob = await new Promise((resolve) =>
      canvas.toBlob(resolve, "image/jpeg", 0.95)
    );
    const editedFile = new File([blob], "edited_image.jpg", {
      type: "image/jpeg",
    });

    const formData = new FormData();
    formData.append("file", editedFile);

    const response = await fetch("/Home/UploadImage", {
      method: "POST",
      body: formData,
    });

    const result = await response.json();
    if (result.success) {
      alert(result.message);
      loadSavedImages();
    } else {
      alert(result.message);
    }
  } catch (error) {
    console.error("Error saving image:", error);
    alert("Có lỗi xảy ra khi lưu ảnh");
  } finally {
    loadingOverlay.style.display = "none";
  }
};

resetFilterBtn.addEventListener("click", resetFilter);
filterSlider.addEventListener("input", updateFilter);

console.log("Adding click event listener to save button");
console.log("Save button element:", saveImgBtn);

// Cập nhật event listener cho nút save
document.querySelector(".save-img").addEventListener("click", (e) => {
  e.preventDefault();
  saveImage();
});

// Thêm hàm updateImage
const updateImage = async () => {
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    alert("Vui lòng chọn ảnh trước khi cập nhật!");
    return;
  }

  if (!currentImageId) {
    alert("Vui lòng chọn một ảnh đã lưu để cập nhật!");
    return;
  }

  const loadingOverlay = document.querySelector(".loading-overlay");
  loadingOverlay.style.display = "flex";

  try {
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    canvas.width = previewImg.naturalWidth;
    canvas.height = previewImg.naturalHeight;

    ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
    ctx.translate(canvas.width / 2, canvas.height / 2);
    if (rotate !== 0) {
      ctx.rotate((rotate * Math.PI) / 180);
    }
    ctx.scale(flipHorizontal, flipVertical);
    ctx.drawImage(
      previewImg,
      -canvas.width / 2,
      -canvas.height / 2,
      canvas.width,
      canvas.height
    );

    const blob = await new Promise((resolve) =>
      canvas.toBlob(resolve, "image/jpeg", 0.95)
    );
    const editedFile = new File([blob], "edited_image.jpg", {
      type: "image/jpeg",
    });

    const formData = new FormData();
    formData.append("file", editedFile);
    formData.append("imageId", currentImageId);

    const response = await fetch("/Home/UpdateImage", {
      method: "POST",
      body: formData,
    });

    const result = await response.json();
    if (result.success) {
      alert(result.message);
      loadSavedImages();
    } else {
      alert(result.message);
    }
  } catch (error) {
    console.error("Error updating image:", error);
    alert("Có lỗi xảy ra khi cập nhật ảnh");
  } finally {
    loadingOverlay.style.display = "none";
  }
};

// Cập nhật event listener cho nút update
document.querySelector(".update-img").addEventListener("click", (e) => {
  e.preventDefault();
  updateImage();
});

document.querySelector(".download-img").addEventListener("click", (e) => {
  e.preventDefault();
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    alert("Vui lòng chọn ảnh trước!");
    return;
  }
  downloadImage();
  // Sẽ thêm chức năng sau
  console.log("Download image clicked");
});

// Thêm hàm downloadImage
const downloadImage = () => {
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    alert("Vui lòng chọn ảnh trước khi tải xuống!");
    return;
  }

  // Tạo canvas với ảnh đã chỉnh sửa
  const canvas = document.createElement("canvas");
  const ctx = canvas.getContext("2d");
  canvas.width = previewImg.naturalWidth;
  canvas.height = previewImg.naturalHeight;

  // Áp dụng các hiệu ứng đã chỉnh sửa
  ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
  ctx.translate(canvas.width / 2, canvas.height / 2);
  if (rotate !== 0) {
    ctx.rotate((rotate * Math.PI) / 180);
  }
  ctx.scale(flipHorizontal, flipVertical);
  ctx.drawImage(
    previewImg,
    -canvas.width / 2,
    -canvas.height / 2,
    canvas.width,
    canvas.height
  );

  // Tạo link tải xuống
  const link = document.createElement("a");
  link.download = "edited_image.jpg";
  link.href = canvas.toDataURL("image/jpeg", 0.95);
  link.click(); // Tự động kích hoạt tải xuống
};

// // Cập nhật event listener cho nút download
// document.querySelector(".download-img").addEventListener("click", (e) => {
//   e.preventDefault();
//   downloadImage();
// });

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
