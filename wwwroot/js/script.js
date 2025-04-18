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

// Hàm lấy imageId từ URL
const getImageIdFromUrl = () => {
  const params = new URLSearchParams(window.location.search);
  const imageId = params.get("imageId");
  if (imageId) {
    currentImageId = parseInt(imageId, 10);
    console.log("Đã lấy imageId từ URL:", currentImageId);
  }
};

// Gọi hàm khi trang được load
document.addEventListener("DOMContentLoaded", () => {
  getImageIdFromUrl();
});

// Hàm hiển thị thông báo
const showToast = (message, type = "success") => {
  const toast = document.createElement("div");
  toast.className = `toast align-items-center text-white bg-${type} border-0 position-absolute top-0 end-0 m-3`;
  toast.setAttribute("role", "alert");
  toast.setAttribute("aria-live", "assertive");
  toast.setAttribute("aria-atomic", "true");
  toast.style.zIndex = "9999";
  toast.innerHTML = `
    <div class="d-flex">
      <div class="toast-body">
        ${message}
      </div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
    </div>
  `;
  document.body.appendChild(toast);
  const bsToast = new bootstrap.Toast(toast, {
    animation: true,
    autohide: true,
    delay: 3000,
  });
  bsToast.show();
  toast.addEventListener("hidden.bs.toast", () => {
    toast.remove();
  });
};

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
            <img src="${image.imagePath}" class="card-img-top" alt="${
            image.imageName
          }" style="height: 150px; object-fit: cover;" crossorigin="anonymous">
            <div class="card-body">
              <p class="card-text small">${
                image.imageName || "Không có tên"
              }</p>
              <button class="btn btn-sm btn-primary select-saved-image" 
                data-url="${image.imagePath}"
                data-id="${image.imageId}"
                data-name="${image.imageName || ""}">
                Choose image
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
          currentImageId = parseInt(btn.dataset.id, 10);
          const imageName = btn.dataset.name;
          console.log("Đã chọn ảnh với ID:", currentImageId, "Tên:", imageName);
          previewImg.crossOrigin = "anonymous";
          previewImg.src = imageUrl;
          previewImg.onload = () => {
            resetFilterBtn.click();
            document.querySelector(".container").classList.remove("disable");
            bootstrap.Modal.getInstance(
              document.getElementById("imageModal")
            ).hide();
          };
          previewImg.onerror = () => {
            showToast("Cannot load image. Please try again.", "error");
          };
        });
      });
    }
  } catch (error) {
    console.error("Error loading saved images:", error);
    showToast("Cant not find image in your cloud", "error");
  }
};

const handleLocalImageSelect = (file) => {
  if (!file) return;
  // Đặt lại currentImageId về null vì đây là ảnh mới
  currentImageId = null;
  previewImg.crossOrigin = "anonymous";
  previewImg.src = URL.createObjectURL(file);
  previewImg.onload = () => {
    resetFilterBtn.click();
    document.querySelector(".container").classList.remove("disable");
    bootstrap.Modal.getInstance(document.getElementById("imageModal")).hide();
    showToast("Image has been selected", "info");
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
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    showToast("Please select an image to save", "warning");
    return;
  }

  const loadingOverlay = document.querySelector(".loading-overlay");
  loadingOverlay.style.display = "flex";

  try {
    // Tạo một ảnh mới để load lại ảnh gốc
    const img = new Image();
    img.crossOrigin = "anonymous";

    await new Promise((resolve, reject) => {
      img.onload = resolve;
      img.onerror = reject;
      img.src = previewImg.src;
    });

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");

    // Điều chỉnh kích thước canvas dựa trên góc xoay
    if (Math.abs(rotate) === 90 || Math.abs(rotate) === 270) {
      canvas.width = img.naturalHeight;
      canvas.height = img.naturalWidth;
    } else {
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;
    }

    ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
    ctx.translate(canvas.width / 2, canvas.height / 2);
    if (rotate !== 0) {
      ctx.rotate((rotate * Math.PI) / 180);
    }
    ctx.scale(flipHorizontal, flipVertical);
    ctx.drawImage(
      img,
      -img.naturalWidth / 2,
      -img.naturalHeight / 2,
      img.naturalWidth,
      img.naturalHeight
    );

    const blob = await new Promise((resolve) =>
      canvas.toBlob(resolve, "image/jpeg", 0.95)
    );

    // Lấy số thứ tự ảnh hiện tại
    const response = await fetch("/Home/GetUserImages");
    const result = await response.json();
    const imageCount = result.success ? result.images.length + 1 : 1;

    const editedFile = new File([blob], `img_${imageCount}.jpg`, {
      type: "image/jpeg",
    });

    const formData = new FormData();
    formData.append("file", editedFile);

    const uploadResponse = await fetch("/Home/UploadImage", {
      method: "POST",
      body: formData,
    });

    const uploadResult = await uploadResponse.json();
    if (uploadResult.success) {
      showToast(uploadResult.message);
      // Cập nhật currentImageId với ID của ảnh mới
      if (uploadResult.imageId) {
        currentImageId = parseInt(uploadResult.imageId, 10);
        console.log("Đã lưu ảnh mới với ID:", currentImageId);
      }
      loadSavedImages();
    } else {
      showToast(uploadResult.message, "danger");
    }
  } catch (error) {
    console.error("Error saving image:", error);
    showToast("An error occurred while saving the image", "danger");
  } finally {
    loadingOverlay.style.display = "none";
  }
};

// Cập nhật event listener cho nút save
document.querySelector(".save-img").addEventListener("click", (e) => {
  e.preventDefault();
  saveImage();
});

// Thêm hàm updateImage
const updateImage = async () => {
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    showToast("Please select an image to update", "warning");
    return;
  }

  console.log(
    "currentImageId trước khi kiểm tra:",
    currentImageId,
    "Kiểu dữ liệu:",
    typeof currentImageId
  );

  // Kiểm tra xem currentImageId có phải là số hợp lệ không
  if (
    currentImageId === null ||
    currentImageId === undefined ||
    isNaN(currentImageId) ||
    currentImageId <= 0
  ) {
    showToast("Please select an image from your album to update", "warning");
    return;
  }

  const loadingOverlay = document.querySelector(".loading-overlay");
  loadingOverlay.style.display = "flex";

  try {
    // Tạo một ảnh mới để load lại ảnh gốc
    const img = new Image();
    img.crossOrigin = "anonymous";

    await new Promise((resolve, reject) => {
      img.onload = resolve;
      img.onerror = reject;
      img.src = previewImg.src;
    });

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");

    // Điều chỉnh kích thước canvas dựa trên góc xoay
    if (Math.abs(rotate) === 90 || Math.abs(rotate) === 270) {
      canvas.width = img.naturalHeight;
      canvas.height = img.naturalWidth;
    } else {
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;
    }

    ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
    ctx.translate(canvas.width / 2, canvas.height / 2);
    if (rotate !== 0) {
      ctx.rotate((rotate * Math.PI) / 180);
    }
    ctx.scale(flipHorizontal, flipVertical);
    ctx.drawImage(
      img,
      -img.naturalWidth / 2,
      -img.naturalHeight / 2,
      img.naturalWidth,
      img.naturalHeight
    );

    const blob = await new Promise((resolve) =>
      canvas.toBlob(resolve, "image/jpeg", 0.95)
    );

    const editedFile = new File([blob], `img_${currentImageId}.jpg`, {
      type: "image/jpeg",
    });

    const formData = new FormData();
    formData.append("file", editedFile);
    formData.append("imageId", currentImageId.toString());

    const response = await fetch("/Home/UpdateImage", {
      method: "POST",
      body: formData,
    });

    const result = await response.json();
    if (result.success) {
      showToast(result.message);
      // Cập nhật lại ảnh hiện tại với URL mới
      if (result.imageUrl) {
        previewImg.src = result.imageUrl;
      }
      loadSavedImages();
    } else {
      showToast(result.message, "danger");
    }
  } catch (error) {
    console.error("Error updating image:", error);
    showToast("An error occurred while updating the image", "danger");
  } finally {
    loadingOverlay.style.display = "none";
  }
};

// Cập nhật event listener cho nút update
document.querySelector(".update-img").addEventListener("click", (e) => {
  e.preventDefault();
  updateImage();
});

// Thêm hàm downloadImage
const downloadImage = async () => {
  if (!previewImg.src || previewImg.src.includes("image-placeholder.jpg")) {
    showToast("Please select an image to download", "warning");
    return;
  }

  try {
    // Tạo một ảnh mới để load lại ảnh gốc
    const img = new Image();
    img.crossOrigin = "anonymous";

    await new Promise((resolve, reject) => {
      img.onload = resolve;
      img.onerror = reject;
      img.src = previewImg.src;
    });

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");

    // Điều chỉnh kích thước canvas dựa trên góc xoay
    if (Math.abs(rotate) === 90 || Math.abs(rotate) === 270) {
      canvas.width = img.naturalHeight;
      canvas.height = img.naturalWidth;
    } else {
      canvas.width = img.naturalWidth;
      canvas.height = img.naturalHeight;
    }

    ctx.filter = `brightness(${brightness}%) saturate(${saturation}%) invert(${inversion}%) grayscale(${grayscale}%)`;
    ctx.translate(canvas.width / 2, canvas.height / 2);
    if (rotate !== 0) {
      ctx.rotate((rotate * Math.PI) / 180);
    }
    ctx.scale(flipHorizontal, flipVertical);
    ctx.drawImage(
      img,
      -img.naturalWidth / 2,
      -img.naturalHeight / 2,
      img.naturalWidth,
      img.naturalHeight
    );

    // Tạo link tải xuống
    const link = document.createElement("a");
    link.download = "edited_image.jpg";
    link.href = canvas.toDataURL("image/jpeg", 0.95);
    link.click(); // Tự động kích hoạt tải xuống
  } catch (error) {
    console.error("Error downloading image:", error);
    showToast("Có lỗi xảy ra khi tải ảnh", "danger");
  }
};

// Cập nhật event listener cho nút download
document.querySelector(".download-img").addEventListener("click", (e) => {
  e.preventDefault();
  downloadImage();
});

// Thêm event listener cho slider
filterSlider.addEventListener("input", updateFilter);

// Thêm event listener cho nút reset
resetFilterBtn.addEventListener("click", resetFilter);
