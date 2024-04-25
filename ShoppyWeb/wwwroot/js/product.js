var dataTable;

$(document).ready(function () {
  loadDataTable();
});

function loadDataTable() {
  dataTable = $("#tblData").DataTable({
    ajax: { url: "/admin/product/getall" },
    columns: [
      { data: "title", width: "20%" },
      {
        data: "imageUrl",
        render: function (imageUrl) {
          return `<img src=${imageUrl} width="100%"></img>`;
        },
        width: "5%",
      },
      { data: "price", width: "5%" },
      { data: "author", width: "10%" },
      { data: "category.name", width: "10%" },
      {
        data: "id",
        render: function (data) {
          return `<div class="w-75 btn-group" role="group">
              <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                <i class="bi bi-pencil-square"></i> Edit
              </a>
              <a onClick=Delete("/admin/product/delete/${data}") class="btn btn-danger mx-2">
                <i class="bi bi-trash-fill"></i> Delete
              </a>
            </div>`;
        },
        width: "20%",
      },
    ],
  });
}

function Delete(url) {
  Swal.fire({
    title: "Are you sure?",
    text: "You won't be able to revert this!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        url: url,
        type: "DELETE",
        success: function (data) {
          toastr.success(data.message);
          dataTable.ajax.reload();
        },
      });
    }
  });
}
