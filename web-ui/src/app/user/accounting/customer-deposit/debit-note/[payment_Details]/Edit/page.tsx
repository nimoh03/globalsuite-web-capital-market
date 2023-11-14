"use client";

import { ROUTES } from "@/lib/routes";
import { useEffect, useState } from "react";
import {
  AutoComplete,
  Button,
  Checkbox,
  DatePicker,
  Form,
  Input,
  Modal,
  Select,
  Spin,
  Tooltip,
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import { BackwardOutlined, SyncOutlined } from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import NotificationService from "@/services/NotificationService";
import { DefaultTime, onSuccessDepositClose } from "@/helpers/helper";
import moment from "moment";

export default observer(function Page({
  params,
}: {
  params: { payment_Details: string };
}) {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [PaymentInstrument, setPaymentInstrument] = useState([]);
  const [allAccount, setAllAccount] = useState([]);
  const [allBranches, setAllBranched] = useState([]);
  const [allProduct, setAllProduct] = useState([]);
   const [allSubBank, setallSubBank] = useState<{
    key : string;
    label: string;
    value: string;
  }[]>([]);
  const [branchCode, setBranchCode] = useState("");
  const [productCode, setproductCode] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [PaymentInstrumentValue, setPaymentInstrumentValue] = useState(0);
  const [CustomerNo, setCustomerNo] = useState("");
  const [defaultBranch,setDefaultBranch] = useState('')
  const [customerBalance, setCustomerBalObject] = useState<{
    customer: {
      Phone: "";
      email: "";
      mobPhone: "";
    };
    balance: "";
  }>({
    customer: {
      Phone: "",
      email: "",
      mobPhone: "",
    },
    balance: "",
  });
  const initialValues = {
    chargeVAT: true,
    vatIsInclusive: true,
    instrumentType: 4,
    acctSubBank: undefined,
    chqueNo: "",
    acctMasBank: undefined,
    doNotChargeBankStampDuty: true,
    code: undefined,
    custNo: undefined,
    ref: "",
    trandesc: "",
    amount: 0,
    branch: "",

    rnDate: DefaultTime,
  };

  const navigateToDebitNote = () => {
    router.push("/user/accounting/customer-deposit/debit-note");
  };

  // const onSuccessDepositClose = () => {
  //   setIsLoading(true);
  //   navigateToDebitNote();
  //   setIsLoading(false);

  // };

  const showModal = async () => {
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(true);
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const handleReset = () => {
    form.setFieldsValue({
      ...initialValues,
      acctMasBank: values.acctMasBank,
      branch:defaultBranch,
      chqueNo: "",
    });
    setCustomerBalObject({
      customer: {
        Phone: "",
        email: "",
        mobPhone: "",
      },
      balance: "",
    });
  };

  const fetch = async () => {
    setIsLoading(true);
    const getInstrumentTypeResponse = await depositStore.getInstrumentTYpe();
    const getAllBranchResponse = await depositStore.getAllBranch();
    const fetchTransactionDetails = await depositStore.getDebitNoteByCode(
      params?.payment_Details.toString(), depositStore.creditNoteStatus || localStorage.getItem('creditNoteStatus')
    );
    const getAllProductResponse = await depositStore.getAllProducts();

    if (
      getInstrumentTypeResponse.kind === "ok" &&
      getAllBranchResponse.kind === "ok" &&
      getAllProductResponse.kind === "ok" &&
      fetchTransactionDetails.kind === "ok"
    ) {
      setPaymentInstrument(getInstrumentTypeResponse.data.result);

      setAllBranched(getAllBranchResponse.data.result);
      setAllProduct(getAllProductResponse.data.result);
      form.setFieldsValue({
        // chargeVAT,
        // vatIsInclusive,

        acctMasBank:
          fetchTransactionDetails.data.result?.productDetail?.productCode,
        branch: fetchTransactionDetails.data.result?.branch?.transNo,
        instrumentType:
          fetchTransactionDetails.data.result?.instrumentType ?? 4,
        code: fetchTransactionDetails.data.result?.debitNo,
        custNo: fetchTransactionDetails.data.result?.customer?.fullName,
        ref: fetchTransactionDetails.data.result?.ref,
        amount: fetchTransactionDetails.data.result?.amount,
        rnDate: moment(fetchTransactionDetails.data.result?.rnDate),
        acctSubBank: fetchTransactionDetails.data.result?.subBank?.accountId,
        chqueNo: fetchTransactionDetails.data.result?.chqueNo,
        trandesc: fetchTransactionDetails.data.result?.tranDesc,
        doNotChargeBankStampDuty:
          fetchTransactionDetails.data.result?.doNotChargeBankStampDuty,
      });
      setCustomerNo(fetchTransactionDetails.data.result.custno);
      setCustomerName(fetchTransactionDetails.data.result.customer.fullName);
      setIsLoading(false);
    } else {
      setIsLoading(false);
    }
  };

  const fetch2 = async () => {
    if (
      values?.branch.length > 1 &&
      values?.acctMasBank.length > 1 &&
      customerName.length > 3
    ) {
      setIsLoading(true);
      const getAllSubBankResponse = await depositStore.getAllSubAcctByProduct(
        customerName.toString(),
        values?.acctMasBank.toString() ?? productCode.toString(),
        values?.branch.toString() ?? branchCode.toString()
      );
      if (getAllSubBankResponse.kind === "ok") {
        setIsLoading(false);
        const newSubArray = getAllSubBankResponse?.data?.result.map(
           (value: { fullName: string; custAID: string }) => {
return ({
  key:value.custAID,
  label: value.fullName,
  value: value.fullName,
})
          }
        );
        setallSubBank(newSubArray);
      } else {
        setIsLoading(false);
      }
      // to get all Account
      const getAllAccountResonse = await depositStore.getAllAccount(
        values?.branch.toString() ?? branchCode.toString()
      );
      if (getAllAccountResonse.kind === "ok") {
        setAllAccount(getAllAccountResonse.data.result);
      }
      //get the customer balance
      if (CustomerNo.length > 0) {
        setIsLoading(true);
        const customerBalResponse = await depositStore.getCustomerBal(
          CustomerNo.toString(),
          values?.acctMasBank ?? productCode.toString()
        );
        if (customerBalResponse.kind === "ok") {
          setCustomerBalObject(customerBalResponse.data.result);
          setIsLoading(false);
        }
      }
    } else return;
  };

  useEffect(() => {
    fetch();
  }, []);

  useEffect(() => {
    fetch2();
  }, [customerName]);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);
  console.log(values);

  const handleSubmit = async () => {
    delete values.branch;
    delete values.recNo;
    delete values.instrumentType;
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.updateDebitNoteTransaction({
        ...values,

        amount: parseInt(values.amount),
        custNo: CustomerNo,
        payDesc : ''
      });
      if (response.kind === "ok") {
        setIsModalOpen(false);
        setIsModalOpen2(true);
      } else{
        setIsLoading(false)
      }
    }
  };

  return (
    <Spin tip="Authenticating...." spinning={isLoading} className="text-[red] ">
      <main className="flex flex-col w-[70%] mx-auto ">
        <div className=" rounded-lg shadow-xl">
          <p className="font-bold text-[16px] gap-2 flex justify-start items-center leading-10 px-2 capitalize bg-[#194bfb] text-white rounded-t-lg">
            <span>
              <Image
                src={images.file_icon}
                alt="icon"
                className="cursor-pointer  text-[10px] "
              />
            </span>
            Customer Debit-Edit
          </p>
          <Form
            form={form}
            initialValues={initialValues}
            className="bg-gray-50 px-8 py-4 rounded border"
            layout="vertical"
          >
            <div className="flex gap-3 ">
              <Form.Item
                name={"code"}
                className="mb-1 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Transaction No.
                  </p>
                }
              >
                <Input type="text" placeholder="DDF22345" />
              </Form.Item>
              <Form.Item
                name={"recNo"}
                className="mb-1  flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Receipt No.
                  </p>
                }
              >
                <Input type="text" placeholder="1234567" />
              </Form.Item>
            </div>
            <div className="flex gap-3">
              <Form.Item
                name={"rnDate"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Transaction Date
                  </p>
                }
                rules={[{ required: true }]}
              >
                <DatePicker className="w-full" format="DD/MM/YYYY" />
              </Form.Item>

              {/* <Form.Item
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Receipt No.
                  </p>
                }
              >
                <Input placeholder="100000" type="number" allowClear/>
              </Form.Item> */}
              <Form.Item
                name={"branch"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Branch A/C
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
               showSearch
               allowClear
               optionFilterProp="children"
                  placeholder="Branch"
                  className="text-gray-600"
                  onSelect={setBranchCode}
                >
                  {allBranches.map(({ transNo, nameWithCode }, index) => (
                    <Select.Option value={transNo} key={index}>
                      {nameWithCode}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </div>
            <div className=" flex flex-col ">
              <div className="flex gap-3">
                <Form.Item
                  name={"acctMasBank"}
                  className="mb-2 flex-1"
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Product Account
                    </p>
                  }
                  rules={[{ required: true }]}
                >
                  <Select
                  showSearch
                  allowClear
                  optionFilterProp="children"
                    placeholder="Product Account"
                    className="text-gray-600"
                    onSelect={setproductCode}
                  >
                    {allProduct.map(({ productCode, productName }, index) => (
                      <Select.Option value={productCode} key={index}>
                        {productName}
                      </Select.Option>
                    ))}
                  </Select>
                </Form.Item>

                <Form.Item
                  name={"custNo"}
                  className="mb-2 flex-1"
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Customer Account
                    </p>
                  }
                  rules={[
                    { required: true, message: "enter at least 4  letter" },
                  ]}
                >
                  <AutoComplete
                    allowClear
                    options={allSubBank}
                   onChange={setCustomerName}
                    onSelect={(value,option)=>{
                      setCustomerNo(option.key);
                    }}
                    placeholder="Enter at least four(4) letter "
                  />
                </Form.Item>
              </div>
              <p className="text-[#128A3E] font-[600] text-[12px] mt-[-8px] w-full text-end px-3">{`Email: ${
                customerBalance.customer?.email
              } | Phone: ${
                customerBalance.customer?.Phone ??
                customerBalance.customer?.mobPhone
              } | Cash Balance: N${customerBalance?.balance.toLocaleString()}`}</p>
            </div>
            <div className="flex gap-3">
              <Form.Item
                name={"acctSubBank"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Account To Debit
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select 
                showSearch
                allowClear
                optionFilterProp="children"
                placeholder="Select Bank" className="text-gray-600">
                  {allAccount.map(({ accountId, accountName }, index) => (
                    <Select.Option value={accountId} key={index}>
                      {accountName}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>

              <Form.Item
                name={"ref"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Reference
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input placeholder="Reference" allowClear/>
              </Form.Item>

              <Form.Item
                name={"amount"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Amount Paid
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input placeholder="100000" type="number" allowClear/>
              </Form.Item>
            </div>

            <div className="flex gap-3">
              <Form.Item
                name={"instrumentType"}
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Paying Instrument
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                showSearch
                allowClear
                optionFilterProp="children"
                  placeholder="lucy"
                  className="text-gray-600"
                  onSelect={setPaymentInstrumentValue}
                >
                  {PaymentInstrument.map(({ id, name }, _) => (
                    <Select.Option value={id} key={id}>
                      {name}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
              {PaymentInstrumentValue === 0 ? null : (
                <Form.Item
                  name={"chqueNo"}
                  className="mb-2 "
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Cheque No
                    </p>
                  }
                  rules={[{ required: true }]}
                >
                  <Input type="text" placeholder="chqueNo" allowClear />
                </Form.Item>
              )}
              <Form.Item
                className="mb-2 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Customer Branch
                  </p>
                }
                // rules={[{ required: true }]}
              >
                <Select
                showSearch
                allowClear
                optionFilterProp="children"
                  placeholder="Customer Branch"
                  className="text-gray-600"
                  onSelect={setBranchCode}
                >
                  {allBranches.map(({ transNo, nameWithCode }, index) => (
                    <Select.Option value={transNo} key={index}>
                      {nameWithCode}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </div>
            <div className="flex-col flex  gap-3">
              <Form.Item
                name={"trandesc"}
                className="mb-1 flex-1 "
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Narration
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input
                allowClear
                  type="text"
                  placeholder="Deposit For Shares"
                  
                />
              </Form.Item>
            </div>
            <div className="flex justify-between capitalize">
              <Form.Item
                name="doNotChargeBankStampDuty"
                className="mb-2"
                valuePropName="checked"
                rules={[{ required: true }]}
              >
                <Checkbox>Do not charge Bank Stamp Duty</Checkbox>
              </Form.Item>
              <Form.Item
                name="chargeVAT"
                className="mb-2"
                valuePropName="checked"
                rules={[{ required: true }]}
              >
                <Checkbox>Check VAT On Transaction</Checkbox>
              </Form.Item>
              <Form.Item
                name="vatIsInclusive"
                className="mb-2"
                valuePropName="checked"
                rules={[{ required: true }]}
              >
                <Checkbox>vat Inclusive</Checkbox>
              </Form.Item>
            </div>

            <div className="flex justify-between items-center gap-5 ">
              <Button
                className="flex justify-center items-center text-white font-[700] text-[16px] bg-[#194BFB]"
                onClick={navigateToDebitNote}
              >
                <BackwardOutlined className="text-[20px]" /> View Unapproved
                Deposit
              </Button>
              <div className="flex justify-end items-center gap-3">
                <Button
                  className=" bg-[#FEE2E2] text-[#DD3333] font-[700] text-[16px]"
                  onClick={handleReset}
                >
                  Cancel
                </Button>
                <Button
                  type="primary"
                  htmlType="submit"
//className = '!bg-[#194BFB] text-[white] font-[600] '
                  onClick={showModal}
                  disabled={
                    values?.amount <= 0 || values?.amount > customerBalance.balance
                  }
                >
                  Save
                </Button>
              </div>
            </div>
          </Form>
        </div>
        <Modal
          open={isModalOpen}
          centered={true}
          okText="save"
          cancelText="Make Changes"
          onOk={handleSubmit}
          onCancel={handleCancel}
          footer={[
            <div
              key={2}
              className=" w-[80%] mx-auto flex justify-center items-center"
            >
              <Button
                key="back"
                htmlType="submit"
                className="w-full bg-[#EDF2F7] text-[#194BFB]"
                onClick={handleCancel}
              >
                Make Changes
              </Button>
              <Button
                key="submit"
                type="primary"
                className="w-full"
                onClick={handleSubmit}
              >
                Save
              </Button>
            </div>,
          ]}
        >
          <div className="w-[80%] mx-auto flex flex-col justify-center items-center text-center mt-8 gap-5">
            <Image src={images.warningModalImage} alt="warning_Modal_Image" />
            <p className="text-[20px] leading-[26px] font-bold">
              Are you sure you want to save?
            </p>
            <p className="text-[14px] leading-[21px] ">
              Make sure you have inserted all the entries correctly or go back
              to make changes.
            </p>
          </div>
        </Modal>
        {/* modal for sucessfully saved deposit */}
        <Modal
          open={isModalOpen2}
          onCancel={() => onSuccessDepositClose(setIsModalOpen2, setIsLoading, handleReset)}
          centered={true}
          footer={[
            <div
              key={4}
              className=" w-[80%] mx-auto flex justify-center items-center capitalize"
            >
              <p className="text-[20px] font-bold leading-[26px] text-center">
                deposit saved sucessfully
              </p>
            </div>,
          ]}
        >
          <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
            <Image src={images.successModalImage} alt="warning_Modal_Image" />
          </div>
        </Modal>
      </main>
    </Spin>
  );
});
