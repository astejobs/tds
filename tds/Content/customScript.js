function showModal(id, type) {
  
    if (type == "General") {

        return;
    }
    $.ajax({
        url: "/transaction/getTransaction",
        data: { id: id },
        method: "get",
        success: function (transaction) {
           
            var date = Date.parse(`${transaction.createDate}`);
            var newDate = date.toString('dd-MM-yyyy');
            $("#transBody").html(`
<div class="row">
        <p class="p-h text-left" ><b>Scheme :</b> ${transaction.scheme} </p>
        <p class="p-h text-right"><b>Contractor:</b> ${transaction.contractor}</p>
    </div>
    <div class="row">

        <p class="p-h text-left"><b>Date : </b>${newDate}</p>
        <p class="p-h text-right"><b>GSTIN : </b>${transaction.gstin}</p>
    </div>
    <hr>
    <table class="table">
  <thead>
    <tr>
      <th scope="col">S>NO</th>
      <th scope="col"></th>
      <th scope="col">Amount</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th scope="row">1</th>
       <td>Amount Paid</td>
      <td>${transaction.amountPaid}</td>
     
    </tr>
    <tr>
      <th scope="row">2</th>
     <td> Deposit</td>
      <td>${Math.round(transaction.deposit)}</td>
    </tr>
 <tr>
      <th scope="row">3</th>
     <td>CGST Amount</td>
      <td>${Math.round(transaction.cgstAmount)}</td>
    </tr>
    <tr>
      <th scope="row">4</th>
     <td>SGST Amount</td>
      <td>${Math.round(transaction.sgstAmount)}</td>
    </tr>
  <tr>
      <th scope="row">5</th>
     <td>IT Amount</td>
      <td>${Math.round(transaction.itAmount)}</td>
    </tr>
  <tr>
      <th scope="row">6</th>
     <td>Labour Cess Amount</td>
      <td>${Math.round(transaction.labourCessAmount)}</td>
    </tr>
 <tr>
      <th scope="row"></th>
     <td> <h4>Net Amount</h4></td>
      <td><h4>${Math.round(transaction.netAmount)}</h4></td>
    </tr>

  </tbody>
</table>

`)
            $("#transFooter").html(`<a class="btn btn-primary" target="_blank" href="/transaction/printtransaction/${transaction.id}">Print Transaction <i class="mdi mdi-printer"></i></a>`);
            $("#transModal").modal("show");

        }
        //,
        //error: function (xhr) {
        //    alert(xhr.responseText);
        //}

    })




}