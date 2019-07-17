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
<div class="row modal-header" style="margin-top:-40px;">
 <a class="btn btn-primary" target="_blank" href="/transaction/printtransaction/${transaction.id}">Print <i class="mdi mdi-printer"></i></a>
  <button type="button" class="close " data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
</div>
<div class="row">
                                            <p  style="font-size:17px; width:100%;display:inline-block; text-align:center;"><b> Work: </b>${transaction.work}</p>
                                            </div>
<div class="row">
       
        <p class="p-h text-left"><b>TV NO:</b> ${transaction.tvNo}</p>
    </div>
<div class="row">
        <p class="p-h text-left" ><b>Scheme :</b> ${transaction.scheme} </p>
        <p class="p-h text-right"><b>Contractor:</b> ${transaction.contractor}</p>
    </div>
    <div class="row">

        <p class="p-h text-left"><b>Date : </b>${newDate}</p>
        <p class="p-h text-right"><b>GSTIN : </b>${transaction.gstin}</p>
    </div>
    <hr>
    <table class="table table-sm">
  <thead>
    <tr  >
      <th class="text-left" scope="col">Title</th>
      <th class="text-right" scope="col">Amount</th>
    </tr>
  </thead>
  <tbody>
    <tr> 
       <td class="text-left">Amount Paid</td>
      <td class="text-right">${transaction.amountPaid}</td>
     
    </tr>
    <tr>
     <td class="text-left"> Deposit</td>
      <td class="text-right">${Math.round(transaction.deposit)}</td>
    </tr>
 <tr>
     <td class="text-left">CGST Amount</td>
      <td  class="text-right">${Math.round(transaction.cgstAmount)}</td>
    </tr>
    <tr>
     <td  class="text-left">SGST Amount</td>
      <td  class="text-right">${Math.round(transaction.sgstAmount)}</td>
    </tr>
  <tr>
     <td  class="text-left">IT Amount</td>
      <td  class="text-right">${Math.round(transaction.itAmount)}</td>
    </tr>
  <tr>
     <td  class="text-left">Labour Cess Amount</td>
      <td  class="text-right">${Math.round(transaction.labourCessAmount)}</td>
    </tr>
 <tr>
     <td  class="text-left"> <h4>Net Amount</h4></td>
      <td  class="text-right"><h4>${Math.round(transaction.netAmount)}</h4></td>
    </tr>

  </tbody>
</table>

`)
           
            $("#transModal").modal("show");

        }
        //,
        //error: function (xhr) {
        //    alert(xhr.responseText);
        //}

    })




}