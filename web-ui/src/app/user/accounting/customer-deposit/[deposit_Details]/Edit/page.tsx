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
import {
  DefaultTime,
  fetch,
  onChangeProduct,
  onSuccessDepositClose,
} from "@/helpers/helper";
import moment from "moment";

export default observer(function Page({
  params,
}: {
  params: { deposit_Details: string };
}) {
  console.log(params);
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
  const [allSubBank, setallSubBank] = useState<
    {
      key: string;
      label: string;
      value: string;
    }[]
  >([]);
  const [branchCode, setBranchCode] = useState("");
  const [productCode, setproductCode] = useState("");
  const [CustomerNo, setCustomerNo] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [PaymentInstrumentValue, setPaymentInstrumentValue] = useState(null);
  const [depositSucessResponse, setDepositSucessResponse] = useState<{
    code: string;
    recNo: string;
  }>({ code: "", recNo: "" });
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
  const [defaultBranch, setDefaultBranch] = useState("");

  const initialValues = {
    instrumentType: 4,
    acctSubBank: undefined,
    branch: "",
    chqueNo: "",
    acctMasBank: undefined,
    doNotChargeBankStampDuty: true,
    code: undefined,
    custNo: undefined,
    ref: "",
    trandesc: "",
    amount: 0,
    rnDate: DefaultTime,
    // recNo: "",
  };
  const navigateToDeposit = () => {
    router.push("/user/accounting/customer-deposit");
  };

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

    const getAllProductResponse = await depositStore.getAllProducts();
    const fetchTransactionDetails = await depositStore.getDepositByCode(
      params.deposit_Details.toString()
    );
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
        instrumentType: fetchTransactionDetails.data.result?.instrumentTypeId,
        acctSubBank: fetchTransactionDetails.data.result?.subBank?.accountId,
        branch: fetchTransactionDetails.data.result?.branch?.transNo,
        chqueNo: fetchTransactionDetails.data.result?.chqueNo,
        acctMasBank:
          fetchTransactionDetails.data.result?.productDetail?.productCode,
        doNotChargeBankStampDuty:
          fetchTransactionDetails.data.result?.doNotChargeBankStampDuty,
        code: fetchTransactionDetails.data.result?.code,
        custNo: fetchTransactionDetails.data.result?.customer?.fullName,
        ref: fetchTransactionDetails.data.result?.ref,
        trandesc: fetchTransactionDetails.data.result?.tranDesc,
        amount: fetchTransactionDetails.data.result?.amount,
        rnDate: moment(fetchTransactionDetails.data.result?.rnDate),
        recNo: fetchTransactionDetails.data.result?.recNo,
      });
      setCustomerNo(fetchTransactionDetails.data.result.custno);
      setCustomerName(fetchTransactionDetails.data.result.customer.fullName);
      setIsLoading(false);
    } else {
      setIsLoading(false);
    }
  };

  console.log(values);
  const fetch2 = async () => {
    if (
      values?.branch.length > 1 &&
      values?.acctMasBank.length > 1 &&
      customerName?.length > 3
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
            return {
              key: value.custAID,
              label: value.fullName,
              value: value.fullName,
            };
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

  const handleSubmit = async () => {
    delete values.branch;
    // console.log(values)
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.updateCustomerTransaction({
        ...values,
        amount: parseInt(values.amount),
        payDesc : '',
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
  console.log(customerName);
  console.log(CustomerNo);
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
            Customer Deposit-Edit
          </p>

          <Form
            form={form}
            initialValues={initialValues}
            className="bg-gray-50 py-4 px-8  rounded border"
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

            <div className="flex gap-3 ">
              <Form.Item
                name={"rnDate"}
                className="mb-1 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Transaction Date
                  </p>
                }
                rules={[{ required: true }]}
              >
                <DatePicker className="w-full" format="DD/MM/YYYY" />
              </Form.Item>

              <Form.Item
                name={"branch"}
                className="mb-1 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Branch A/C
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
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
                  className="mb-[6px] flex-1"
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
                  className="mb-[6px] flex-1"
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Subsidiary Account
                    </p>
                  }
                  rules={[
                    { required: true, message: "Enter at Least (4)  Letters" },
                  ]}
                >
                  <AutoComplete
                    options={allSubBank}
                    allowClear
                    onChange={setCustomerName}
                    onSelect={(value, option) => {
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
                className="mb-1 flex-1"
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
                  placeholder="Cash/Bank/A/C"
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
                className="mb-1 flex-1"
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
                name={"instrumentType"}
                className="mb-1 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Paying Instrument
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                  allowClear
                  showSearch
                  optionFilterProp="children"
                  placeholder="Cash"
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
              {PaymentInstrumentValue === 0 ||
              values?.instrumentType === 0 ? null : (
                <Form.Item
                  name={"chqueNo"}
                  className="mb-1 "
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Cheque No
                    </p>
                  }
                >
                  <Input type="text" placeholder="Cheque Number" />
                </Form.Item>
              )}
              <Form.Item
                name={"ref"}
                className="mb-1 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Reference
                  </p>
                }
              >
                <Input placeholder="Reference...." />
              </Form.Item>
            </div>

            <div className="flex gap-3">
              <Form.Item
                name={"trandesc"}
                className="mb-1 flex-1"
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
          onCancel={() => navigateToDeposit()}
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
