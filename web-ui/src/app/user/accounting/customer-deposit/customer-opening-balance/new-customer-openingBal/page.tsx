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
  Radio,
  Select,
  Spin,
  Tooltip,
} from "antd";
import type { RadioChangeEvent } from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import { BackwardOutlined, SyncOutlined } from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import { DefaultTime, onSuccessDepositClose } from "@/helpers/helper";

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
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
  const [radioValue, setRadioValue] = useState("C");
  const [CustomerNo, setCustomerNo] = useState("");
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
  const [defaultBranch,setDefaultBranch] = useState('')
  const initialValues = {
    code: "",
    custNo: "",
    ref: "",
    transDesc: "",
    amount: 0,
    debCred: "",
    rnDate: DefaultTime,
    branch : '',
    acctMasBank : ''
  };

  const navigateToCustomerOpeningBAl = () => {
    router.push("/user/accounting/customer-deposit/customer-opening-balance");
  };

  const showModal = async () => {
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(true);
    }
  };
  const onChangeRadio = (e: RadioChangeEvent) => setRadioValue(e.target.value);

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const handleReset = () => {
    form.setFieldsValue({
      ...initialValues,
      acctMasBank: values.acctMasBank,
      branch: defaultBranch,
      
    });
    setCustomerBalObject({
      customer: {
        Phone: "",
        email: "",
        mobPhone: "",
      },
      balance: "",})
  };

  const fetch = async () => {
    setIsLoading(true);

    const getAllBranchResponse = await depositStore.getAllBranch();

    const getAllProductResponse = await depositStore.getAllProducts();

    if (
      getAllBranchResponse.kind === "ok" &&
      getAllProductResponse.kind === "ok"
    ) {
      setAllBranched(getAllBranchResponse.data.result);
      setAllProduct(getAllProductResponse.data.result);
      form.setFieldsValue({
        ...values,
        acctMasBank: getAllProductResponse.data.result[0].productCode,
        branch: getAllBranchResponse.data.result[0].transNo,
      });
      setDefaultBranch(getAllBranchResponse.data.result[0].transNo)
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
      }else {
        setIsLoading(false);
      }
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
    delete values.acctMasBank;
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.createNewOpeningBAl({
        ...values,
        amount: parseInt(values.amount),
        code: "",
        ref: productCode,
        debCred: radioValue,
        custNo : CustomerNo
      });
      if (response.kind === "ok") {
        setIsModalOpen(false);
        setIsModalOpen2(true);
      }
    } else {
      setIsModalOpen(false);
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
            Customer Opening Balance-New
          </p>

          <Form
            form={form}
            initialValues={initialValues}
            className="bg-gray-50 p-8 rounded border"
            layout="vertical"
          >
            <div className="flex gap-3">
              <Form.Item
              name={'branch'}
                className="mb-3 flex-1"
                label={
                  <p className="font-[600] leading-[20px] text-[16px] ">
                    Customer Branch
                  </p>
                }
                rules={[{ required: true }]}
              >
                <Select
                  allowClear
                  showSearch
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
              <Form.Item
              name={'branch'}
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
                  showSearch
                  optionFilterProp="children"
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
            </div>
            <div className=" flex flex-col ">
              <div className="flex gap-3">
                <Form.Item
                  name={"acctMasBank"}
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
                name={"amount"}
                className="mb-3 flex-1"
                label={<p className="font-[600] leading-[20px] text-[16px] ">Amount Paid</p>}
                rules={[{ required: true }]}
              >
                <Input placeholder="100000" type="number" allowClear />
              </Form.Item>
            </div>

            <Radio.Group onChange={onChangeRadio} value={radioValue}>
              <Radio value={"C"}>Credit</Radio>
              <Radio value={"D"}>Debit</Radio>
            </Radio.Group>

            <Form.Item
              name={"trandesc"}
              className="mb-3 flex-1"
              label={<p className="font-[600] leading-[20px] text-[16px] ">Narration</p>}
              rules={[{ required: true }]}
            >
              <Input type="text"  placeholder="Deposit For Shares"
                  allowClear />
            </Form.Item>

            <div className="flex justify-between items-center gap-5 ">
              <Button
                className="flex justify-center items-center text-white font-[700] text-[16px] bg-[#194BFB]"
                onClick={navigateToCustomerOpeningBAl}
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
          onCancel={() =>  onSuccessDepositClose(setIsModalOpen2, setIsLoading, handleReset)}
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
