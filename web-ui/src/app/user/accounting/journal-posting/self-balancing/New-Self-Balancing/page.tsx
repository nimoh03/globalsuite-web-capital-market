"use client";

import { useEffect, useState } from "react";
import {
 
  Button,
  Form,
  Input,
  Modal,
  Select,
  Spin,
  Tooltip,
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import { SyncOutlined } from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [allBranches, setAllBranched] = useState([]);
  const [customerAccount, setCustomerAccount] = useState([]);
  const [branchCode, setBranchCode] = useState("");
  const [receiverBranch, setReceiverBranch] = useState([]);
  const [receiverAccount, setReceiverAccount] = useState([]);
  const [PaymentInstrument, setPaymentInstrument] = useState([]);

  const [receiverBranchCode, setReceiverBranchCode] = useState("");

  const initialValues = {
    transNo: "",
    mainAcctID: "",
    conAccountID: "",
    description: "",
    ref: "",
    amount: 0,
    instrumentType: 0,
  };

  const navigateToCustomerTransfer = () => {
    router.push("/user/accounting/customer-deposit/customer-transfer/");
  };

  const onSuccessDepositClose = () => {
    setIsLoading(true);
    navigateToCustomerTransfer();
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
    form.resetFields();
  };

  const fetchAll = async () => {
    setIsLoading(true);
    const getInstrumentTypeResponse = await depositStore.getInstrumentTYpe();

    const getAllBranchResponse = await depositStore.getAllBranch();

    if (getAllBranchResponse.data.statusCode === 200) {
      setPaymentInstrument(getInstrumentTypeResponse.data.result);
      setAllBranched(getAllBranchResponse.data.result);
      setReceiverBranch(getAllBranchResponse.data.result);
      setIsLoading(false);
    } else {
      setIsLoading(false);
    }
  };

  const fetchCustomerAccount = async () => {
    if (branchCode.length > 1) {
      setIsLoading(true);
      const getAllAccountResonse = await depositStore.getAllAccount(
        branchCode.toString()
      );
      if (getAllAccountResonse.kind === "ok") {
        setCustomerAccount(getAllAccountResonse.data.result);
        setIsLoading(false);
      }
    }
  };

  const fetchReceiverAccount = async () => {
    if (receiverBranchCode.length > 1) {
      setIsLoading(true);
      const getAllAccountResonse = await depositStore.getAllAccount(
        receiverBranchCode.toString()
      );
      if (getAllAccountResonse.kind === "ok") {
        setReceiverAccount(getAllAccountResonse.data.result);
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchAll();
  }, []);

  useEffect(() => {
    fetchCustomerAccount();
  }, [branchCode]);

  useEffect(() => {
    fetchReceiverAccount();
  }, [receiverBranchCode]);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);

  const handleSubmit = async () => {
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.createJournalSelfBalancing({
        ...values,
        amount: parseInt(values.amount),
        transNo: "",
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
      <main className="flex flex-col w-[95%] mx-auto">
        <div className="flex justify-end">
          <Button
            className="mb-5"
            type="primary"
            onClick={navigateToCustomerTransfer}
          >
            view saved
          </Button>
        </div>

        <Form
          form={form}
          initialValues={initialValues}
          className="bg-gray-50 p-8 rounded border"
          layout="vertical"
        >
          <div className="flex justify-end items-end  outline-none ">
            <Tooltip title="refresh" placement="bottom">
              <SyncOutlined
                onClick={handleReset}
                className="flex justify-end items-end cursor-pointer outline-none  "
              />
            </Tooltip>
          </div>

          <div className="flex gap-3">
            <Form.Item
              className="mb-3 flex-1"
              label="Branch Debit A/C "
              rules={[{ required: true }]}
            >
              <Select
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
              className="mb-3 flex-1"
              label="Branch Credit A/C"
              rules={[{ required: true }]}
            >
              <Select
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
          <div className="flex gap-3">
            <Form.Item
              name={"mainAcctID"}
              className="mb-3 flex-1"
              label="Account to Debit"
              rules={[{ required: true }]}
            >
              <Select placeholder="Select Bank" className="text-gray-600">
                {customerAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"amount"}
              className="mb-3 flex-1"
              label="Amount"
              rules={[{ required: true }]}
            >
              <Input placeholder="100000" type="number" />
            </Form.Item>
          </div>

          <div>
            <p className="font-[600] text-[16px] leading-[20px] capitalize py-5 text-[#232B38]">
              Credit Account
            </p>
          </div>

          <div className="flex gap-3">
            <Form.Item
              name={"conAccountID"}
              className="mb-3 flex-1"
              label="Account to Credit"
              rules={[{ required: true }]}
            >
              <Select placeholder="Select Bank" className="text-gray-600">
                {receiverAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="flex gap-3">
            <Form.Item
              name={"instrumentType"}
              className="mb-3 flex-1"
              label="Paying Instrument"
              rules={[{ required: true }]}
            >
              <Select placeholder="lucy" className="text-gray-600">
                {PaymentInstrument.map(({ id, name }, _) => (
                  <Select.Option value={id} key={id}>
                    {name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"ref"}
              className="mb-3 flex-1"
              label="Payment Voucher"
              rules={[{ required: true }]}
            >
              <Input />
            </Form.Item>
          </div>

          <div className="flex gap-3">
            <Form.Item
              name={"description"}
              className="mb-3 flex-1"
              label="description"
              rules={[{ required: true }]}
            >
              <Input type="text" placeholder="for holiday...." />
            </Form.Item>
          </div>

          <div className="flex justify-end mt-10">
            <Button type="primary" htmlType="submit" onClick={showModal}>
              Save
            </Button>
          </div>
        </Form>

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
          <div className="w-[80%] mx-auto justify-center items-center text-center mt-8 gap-5">
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
          onCancel={() => onSuccessDepositClose()}
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
