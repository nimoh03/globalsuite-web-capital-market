"use client";

import { ROUTES } from "@/lib/routes";
import { useEffect, useState } from "react";
import {
  AutoComplete,
  Button,
  DatePicker,
  Form,
  Input,
  Modal,
  Select,
  Spin,
  Tooltip,
  Checkbox,
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import { BackwardOutlined, SyncOutlined } from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import NotificationService from "@/services/NotificationService";
import type { CheckboxChangeEvent } from "antd/es/checkbox";
import {
  DefaultTime,
  fetch,
  onChangeProduct,
  onSuccessDepositClose,
} from "@/helpers/helper";

export default observer(function Page() {
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
  const [allSubBank, setallSubBank] =  useState<{
    key : string;
    label: string;
    value: string;
  }[]>([]);
  const [branchCode, setBranchCode] = useState("");
  const [productCode, setproductCode] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [PaymentInstrumentValue, setPaymentInstrumentValue] = useState(0);

  const [CustomerNo, setCustomerNo] = useState("");
  const [customerBalance, setCustomerBal] = useState<{
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
  const [depositSucessResponse, setDepositSucessResponse] = useState<{
    code: string;
    recNo: string;
  }>({ code: "", recNo: "" });
  const [defaultBranch,setDefaultBranch] = useState('')

  const initialValues = {
    realProduct: "",
    instrumentType: 4,
    code: "",
    custNo: "",
    ref: "",
    amount: 0,
    rnDate: DefaultTime,
    acctSubBank: "",
    chequeNo: "",
    acctMasBank: "",
    payDesc: "",
    transdesc: "Deposit For Shares",
    doNotChargeBankStampDuty: true,
    branch: "",
  };
  console.log(values);
  const navigateToDeposit = () => {
    router.push("/user/accounting/customer-deposit/payments");
  };

  // const onSuccessDepositClose = () => {
  //   setIsLoading(true);
  //   navigateToDeposit();
  // };

  const showModal = async () => {
    if (values?.amount < 0 || values?.amount > customerBalance.balance) {
      return;
    }
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
      branch: defaultBranch,
      chequeNo: "",
    });
    setCustomerBal({
      customer: {
        Phone: "",
        email: "",
        mobPhone: "",
      },
      balance: "",
    });
  };

  // const fetch = async () => {
  //   setIsLoading(true);
  //   const getInstrumentTypeResponse = await depositStore.getInstrumentTYpe();
  //   const getAllBranchResponse = await depositStore.getAllBranch();

  //   const getAllProductResponse = await depositStore.getAllProducts();

  //   if (
  //     getInstrumentTypeResponse.kind === "ok" &&
  //     getAllBranchResponse.kind === "ok" &&
  //     getAllProductResponse.kind === "ok"
  //   ) {
  //     setPaymentInstrument(getInstrumentTypeResponse.data.result);

  //     setAllBranched(getAllBranchResponse.data.result);
  //     setAllProduct(getAllProductResponse.data.result);
  //     form.setFieldsValue({
  //       ...values,
  //       acctMasBank: getAllProductResponse.data.result[0].productCode,
  //       branch: getAllBranchResponse.data.result[0].transNo,
  //     });
  //     setIsLoading(false);
  //   } else {
  //     setIsLoading(false);
  //   }
  // };

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
          (value: { fullName: string; custAID: string }) => ({
            key : value.custAID,
            label: value.fullName,
            value: value.fullName,
          })
        );
        setallSubBank(newSubArray);
      }

      const getAllAccountResonse = await depositStore.getAllAccount(
        values?.branch.toString() ?? branchCode.toString()
      );
      setAllAccount(getAllAccountResonse.data.result);
      //get the customer balance
      if (CustomerNo.length > 0) {
        setIsLoading(true);
        const customerBalResponse = await depositStore.getCustomerBal(
          CustomerNo.toString(),
          values?.acctMasBank ?? productCode.toString()
        );
        if (customerBalResponse.kind === "ok") {
          setIsLoading(false);
          setCustomerBal(customerBalResponse.data.result);
        } else {
          setIsLoading(false);
        }
      }
    } else return;
  };

  useEffect(() => {
    fetch(
      setPaymentInstrument,
      setAllBranched,
      setAllProduct,
      form,
      values,
      setIsLoading,
      depositStore,
      setDefaultBranch
    );
  }, []);

  useEffect(() => {
    console.log(branchCode);
    fetch2();
  }, [customerName]);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);

  const handleSubmit = async () => {
    delete values.branch;

    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.createNewPayment({
        ...values,
        amount: parseInt(values.amount),
        realProduct: "",
        code: "",
        payDesc: "",
        custNo : CustomerNo
      });
      if (response.kind === "ok") {
        setDepositSucessResponse(response.data.result);
        setIsModalOpen(false);
        setIsModalOpen2(true);
      } else {
        setIsLoading(false);
      }
    }
  };
  console.log(values?.instrumentType);
  return (
    <Spin tip="Authenticating...." spinning={isLoading} className="text-[red] ">
      <main className="flex flex-col w-[70%] mx-auto mt-1">
        <div className=" rounded-lg shadow-xl">
          <p className="font-bold text-[16px] gap-2 flex justify-start items-center leading-10 px-2 capitalize bg-[#194bfb] text-white rounded-t-lg">
            <span>
              <Image
                src={images.file_icon}
                alt="icon"
                className="cursor-pointer  text-[10px] "
              />
            </span>
            Customer Payment-New
          </p>

          <Form
            form={form}
            initialValues={initialValues}
            className="bg-gray-50 py-2 px-8  rounded border"
            layout="vertical"
          >
            <div className="flex gap-3">
              <Form.Item
                name={"rnDate"}
                className="mb-3 flex-1"
                label={<p className="font-[600] leading-[20px] text-[16px] ">Transaction  Date</p>}
                rules={[{ required: true }]}
              >
                <DatePicker className="w-full" format="DD/MM/YYYY" />
              </Form.Item>

              <Form.Item
                name={"branch"}
                className="mb-3 flex-1"
                label={<p className="font-[600] leading-[20px] text-[16px] ">Branch A/C</p>}
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
            <div>
              <div className="flex gap-3">
                <Form.Item
                  name={"acctMasBank"}
                  className="mb-3 flex-1"
                  label={<p className="font-[600] leading-[20px] text-[16px] ">Product Account</p>}
                  rules={[{ required: true }]}
                >
                  <Select
                    showSearch
                    allowClear
                    optionFilterProp="children"
                    placeholder="Product Account"
                    className="text-gray-600"
                    onChange={() =>
                      onChangeProduct(form, setallSubBank, values)
                    }
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
                  className="mb-3 flex-1"
                  label={<p className="font-[600] leading-[20px] text-[16px] ">Subsidiary Account</p>}

                  rules={[
                    { required: true, message: "enter at least 4  letter" },
                  ]}
                >
                  <AutoComplete
                    options={allSubBank}
                    allowClear
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
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Cash/Bank/A/C
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                  showSearch
                  allowClear
                  optionFilterProp="children"
                  placeholder="Select Bank"
                  className="text-gray-600"
                >
                  {allAccount.map(({ accountId, accountName }, index) => (
                    <Select.Option value={accountId} key={index}>
                      {accountName}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
              <Form.Item
                name={"amount"}
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Amount Paid
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input placeholder="100000" type="number" min={0} />
              </Form.Item>
            </div>
            <div className="flex gap-3">
              <Form.Item
                name={"ref"}
                className="mb-1 flex-1"
                  label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Reference
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input placeholder="Reference...." />
              </Form.Item>

              <Form.Item
                name={"instrumentType"}
                className="mb-3 flex-1"
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
                  placeholder="Instrument Type"
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
              {PaymentInstrumentValue !== 0 || values?.instrumentType !== 0 ? (
                <Form.Item
                  name={"chequeNo"}
                  className="mb-1 "
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Cheque No
                    </p>
                  }
                >
                  <Input type="text" placeholder="Cheque Number" />
                </Form.Item>
              ) : null}
            </div>

            <div className="flex gap-3">
              <Form.Item
                name={"transdesc"}
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Narration
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input
                  type="text"
                  placeholder="Deposit For Shares"
                  allowClear
                />
              </Form.Item>
            </div>
            <div>
              <Form.Item
                name="doNotChargeBankStampDuty"
                className="mb-1 flex-1"
                valuePropName="checked"
                rules={[{ required: true }]}
              >
                <Checkbox>Do not charge Bank Stamp Duty</Checkbox>
              </Form.Item>
            </div>

            <div className="flex justify-between items-center gap-5 ">
              <Button
                className="flex justify-center items-center text-white font-[700] text-[16px] bg-[#194BFB]"
                onClick={navigateToDeposit}
              >
                <BackwardOutlined className="text-[20px]" /> View Saved Deposit
              </Button>
              <div className="flex justify-end items-center gap-3">
                <Button className=" bg-[#FEE2E2] text-[#DD3333] font-[700] text-[16px]" onClick={handleReset}>
                  Cancel
                </Button>
                <Button
                  type="primary"
//                   htmlType="submit"
// className = '!bg-[#194BFB] text-[white] font-[600] '
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
            <div key={2} className=" w-[80%] mx-auto flex justify-center items-center">
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
          onCancel={() =>
            onSuccessDepositClose(setIsModalOpen2, setIsLoading, handleReset)
          }
          centered={true}
          footer={[
            <div
              key={4}
              className=" w-full flex flex-col justify-center items-center capitalize"
            >
              <p className="text-[20px] font-bold leading-[26px] text-center capitalize">
                deposit saved sucessfully
                <br />
              </p>
              <div className="flex flex-col justify-start items-start">
                <p className="text-sm font-[400]">
                  Receipt No : {depositSucessResponse.recNo}
                </p>
                <p className="text-sm font-[400]">
                  Transaction No : {depositSucessResponse.code}
                </p>
              </div>
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
