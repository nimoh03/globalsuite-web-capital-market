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
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import { BackwardOutlined, SyncOutlined } from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import NotificationService from "@/services/NotificationService";
import { DefaultTime, onSuccessDepositClose } from "@/helpers/helper";
interface DataType {
  customer: {
    Phone: "";
    email: "";
    mobPhone: "";
  };
  balance: "";
}

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  // const {amount} = values && value;
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [allBranches, setAllBranched] = useState([]);
  const [allProduct, setAllProduct] = useState([]);
  const [customerAccount, setCustomerAccount] = useState<{
    key : string;
    label: string;
    value: string;
  }[]>([]);
  const [branchCode, setBranchCode] = useState("");
  const [productCode, setproductCode] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [receiverProductAcct, setReceiverProductAcct] = useState([]);
  const [receiverBranch, setReceiverBranch] = useState([]);
  const [receiverAccount, setReceiverAccount] = useState<{
    key : string;
    label: string;
    value: string;
  }[]>([]);
  const [receiverName, setReceiverName] = useState("");
  const [receiverBranchCode, setReceiverBranchCode] = useState("");
  const [receiverProductCode, setReceiverproductCode] = useState("");
  const [customerBalance, setCustomerBal] = useState<DataType>({
    customer: {
      Phone: "",
      email: "",
      mobPhone: "",
    },
    balance: "",
  });
  const [receiverBalance, setReceiverBal] = useState<DataType>({
    customer: {
      Phone: "",
      email: "",
      mobPhone: "",
    },
    balance: "",
  });
  const [isError, setIsError] = useState(false);
  const [CustomerNo, setCustomerNo] = useState("");
  const [receiverNo, setReceiverNo] = useState("");
  const [defaultBranch,setDefaultBranch] = useState('')

  console.log(values);

  const initialValues = {
    transNo: "",
    rCustAID: "",
    rProduct: "",
    ref: "",
    amount: 0,
    description: "",
    tCustAID: "",
    tProduct: "",
    transNoRev: "",
    rnDate: DefaultTime,
    branch: "",
  };



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
      rProduct: values.rProduct,
      
      tProduct: values.tProduct,
      branch:defaultBranch,
      rnDate: DefaultTime,
    });
    setCustomerBal({
      customer: {
        Phone: "",
        email: "",
        mobPhone: "",
      },
      balance: "",
    });
    setReceiverBal({
      customer: {
        Phone: "",
        email: "",
        mobPhone: "",
      },
      balance: "",
    });
  };

  const fetchAll = async () => {
    setIsLoading(true);

    const getAllBranchResponse = await depositStore.getAllBranch();
    const getAllProductResponse = await depositStore.getAllProducts();

    if (
      getAllBranchResponse.kind === "ok" &&
      getAllProductResponse.kind === "ok"
    ) {
      setAllBranched(getAllBranchResponse.data.result);
      setAllProduct(getAllProductResponse.data.result);
      setReceiverProductAcct(getAllProductResponse.data.result);
      setReceiverBranch(getAllBranchResponse.data.result);
      form.setFieldsValue({
        ...values,
        rProduct: getAllProductResponse.data.result[0].productCode,
        tProduct: getAllProductResponse.data.result[0].productCode,
        branch: getAllBranchResponse.data.result[0].transNo,
      });
      setDefaultBranch(getAllBranchResponse.data.result[0].transNo)
      setIsLoading(false);
    } else {
      setIsLoading(false);
    }
  };

  const fetchCustomerDetails = async () => {
    if (
      values?.branch.length > 1 &&
      values?.rProduct.length > 1 &&
      customerName.length > 3
    ) {
      setIsLoading(true);
      const getcustomerAccountResponse =
        await depositStore.getAllSubAcctByProduct(
          customerName.toString(),
          values?.rProduct.toString() ?? productCode.toString(),
          values?.branch.toString() ?? branchCode.toString()
        );
      if (getcustomerAccountResponse.kind === "ok") {
        setIsLoading(false);
        const newSubArray = getcustomerAccountResponse?.data?.result.map(
           (value: { fullName: string; custAID: string }) => {
return ({
  key:value.custAID,
  label: value.fullName,
  value: value.fullName,
})
          }
        );
        setCustomerAccount(newSubArray);
      } else {
        setIsLoading(false);
      }
    }
    if(CustomerNo.length > 0) {
      setIsLoading(true);
      const customerBalResponse = await depositStore.getCustomerBal(
        values?.rCustAID ?? CustomerNo.toString(),
          values?.rProduct ?? productCode.toString()
      );
      if (customerBalResponse.kind === "ok") {
        setCustomerBal(customerBalResponse.data.result);
        setIsLoading(false)
      }else {
        setIsLoading(false);
      }
    } else return;
  };

  const fetchReceiverDetails = async () => {
    if (
      values?.branch.length > 1 &&
      values?.tProduct.length > 1 &&
      receiverName.length > 3
    ) {
      setIsLoading(true);
      const receiverAccountResponse = await depositStore.getAllSubAcctByProduct(
        receiverName.toString(),
          values?.tProduct.toString() ?? productCode.toString(),
          values?.branch.toString() ?? branchCode.toString()
      );
      if (receiverAccountResponse.kind === "ok") {
        setIsLoading(false);
        const newSubArray = receiverAccountResponse?.data?.result.map(
           (value: { fullName: string; custAID: string }) => {
return ({
  key:value.custAID,
  label: value.fullName,
  value: value.fullName,
})
          }
        );
        setReceiverAccount(newSubArray);
      } else {
        setIsLoading(false);
      }
    }
    if (receiverNo.length > 0) {
      setIsLoading(true);
      const receiverAccountResponse = await depositStore.getCustomerBal(
        values?.tCustAID ?? receiverNo.toString(),
        values?.tProduct ?? productCode.toString()
      );
      if (receiverAccountResponse.kind === "ok") {
        setReceiverBal(receiverAccountResponse.data.result);
        setIsLoading(false)
      }else {
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchAll();
  }, []);

  useEffect(() => {
    fetchCustomerDetails();
  }, [customerName]);

  useEffect(() => {
    fetchReceiverDetails();
  }, [receiverName]);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);

  const handleSubmit = async () => {
    delete values.branch;
    delete values.rnDate;
    if (values && values.amount > customerBalance) {
      setIsError(!isError);
    }
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.createCustomerTransfer({
        ...values,
        amount: parseInt(values.amount),
        transNo: "",
        transNoRev: "",
        rCustAID : CustomerNo,
        tCustAID : receiverNo,
      });
      if (response.kind === "ok") {
        setIsModalOpen(false);
        setIsModalOpen2(true);
      } else {
        setIsLoading(false);
      }
    }
  };

  return (
    <Spin tip="Authenticating...." spinning={isLoading} className="text-[red] ">
      <main className="flex flex-col w-[70%] mx-auto">
        <div className=" rounded-lg shadow-xl">
          <p className="font-bold text-[16px] gap-2 flex justify-start items-center leading-10 px-2 capitalize bg-[#194bfb] text-white rounded-t-lg">
            <span>
              <Image
                src={images.file_icon}
                alt="icon"
                className="cursor-pointer  text-[10px] "
              />
            </span>
            Customer Transfer-New
          </p>

          <Form
            form={form}
            initialValues={initialValues}
            className="bg-gray-50 py-4 px-8 rounded border"
            layout="vertical"
          >
            <div className="flex gap-3">
              <Form.Item
                name={"rnDate"}
                className="mb-3 flex-1"
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
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Branch From
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                  allowClear
                  optionFilterProp="children"
                  showSearch
                  placeholder="Branch from"
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
              <Form.Item
                name={"branch"}
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Branch To
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                  allowClear
                  optionFilterProp="children"
                  showSearch
                  placeholder="Branch to"
                  className="text-gray-600"
                  onSelect={setReceiverBranchCode}
                >
                  {receiverBranch.map(({ transNo, nameWithCode }, index) => (
                    <Select.Option value={transNo} key={index}>
                      {nameWithCode}
                    </Select.Option>
                  ))}
                </Select>
              </Form.Item>
            </div>
            <div>
              <p className="font-[600] text-[16px] leading-[20px] capitalize py-3 text-[#232B38]">
                customer account transfer from
              </p>
            </div>
            <div className=" flex flex-col ">
              <div className="flex gap-3">
                <Form.Item
                  name={"rProduct"}
                  className="mb-2 flex-1"
                  label={
                    <p className="font-[600] leading-[20px] text-[16px] ">
                      Product Account
                    </p>
                  }
                  rules={[{ required: true }]}
                >
                  <Select
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
                  name={"rCustAID"}
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
                    options={customerAccount}
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

            <div>
              <p className="font-[600] text-[16px] leading-[20px] capitalize py-3 text-[#232B38]">
                Customer Account to transfer to:
              </p>
            </div>
            <div>
              <div className="flex gap-3">
                <Form.Item
                  name={"tProduct"}
                  className="mb-3 flex-1"
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
                    onSelect={setReceiverproductCode}
                  >
                    {receiverProductAcct.map(
                      ({ productCode, productName }, index) => (
                        <Select.Option value={productCode} key={index}>
                          {productName}
                        </Select.Option>
                      )
                    )}
                  </Select>
                </Form.Item>

                <Form.Item
                  name={"tCustAID"}
                  className="mb-3 flex-1"
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
                    showSearch
                    options={receiverAccount}
                    onChange={setReceiverName}
                    onSelect={(value,option)=>{
                      setReceiverNo(option.key);
                    }}
                    placeholder="Enter at least four(4) letter "
                  />
                </Form.Item>
              </div>
              <p className="text-[#128A3E] font-[600] text-[12px] mt-[-8px] w-full text-end px-3">{`Email: ${
                receiverBalance.customer?.email
              } | Phone: ${
                receiverBalance.customer?.Phone ??
                receiverBalance.customer?.mobPhone
              } | Cash Balance: N${receiverBalance?.balance}`}</p>
            </div>
            <div className="flex gap-3">
              <div className="flex flex-col text-[red] font-[600]">
                {isError ? <p>insufficient Balance</p> : null}
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
                  <Input placeholder="100000" type="number" allowClear />
                </Form.Item>
              </div>

              <Form.Item
                name={"ref"}
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Reference
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Input placeholder="Reference" allowClear />
              </Form.Item>

              <Form.Item
                name={"description"}
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

            <div className="flex justify-end mt-10"></div>
            <div className="flex justify-between items-center gap-5 ">
              <Button
                className="flex justify-center items-center text-white font-[700] text-[16px] bg-[#194BFB]"
                // onClick={navigateToDeposit}
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
                    (values && values.amount > customerBalance.balance) ||
                    customerName === receiverName || values.amount <= 0
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
                disabled={values && values.amount > customerBalance}
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
              className=" w-full flex justify-center items-center capitalize"
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
