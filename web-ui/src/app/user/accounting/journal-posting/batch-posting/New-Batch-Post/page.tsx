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
  Space,
  Spin,
  Tooltip,
} from "antd";
import { observer } from "mobx-react-lite";
import { useRouter } from "next/navigation";
import {
  MinusCircleOutlined,
  PlusOutlined,
  SyncOutlined,
} from "@ant-design/icons";
import { images } from "@/theme";
import Image from "next/image";
import { useStores } from "@/hooks/use-store";
import NotificationService from "@/services/NotificationService";

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  // const {amount} = values && value;
  const router = useRouter();

  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [branchCode, setBranchCode] = useState("");
  const [allBranches, setAllBranched] = useState([]);
  const [customerAccount, setCustomerAccount] = useState([]);

  const initialValues = {
    effectiveDate: "",
    transactions: [
      {
        accountId: "",
        description: "",
        debit: null,
        credit: null,
      },
    ],
  };
  console.log(values);
  const navigateToBatchHome = () => {
    router.push("/user/accounting/journal-posting/batch-posting");
  };

  const onSuccessDepositClose = () => {
    setIsLoading(true);
    navigateToBatchHome();
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

  const fetchAllAccount = async () => {
    setIsLoading(true);
    const getAllBranchResponse = await depositStore.getAllBranch();
    if (getAllBranchResponse.kind === "ok") {
      setAllBranched(getAllBranchResponse.data.result);
    }
    if (branchCode.length > 1) {
      setIsLoading(true);
      const getAllAccountResonse = await depositStore.getAllAccount(
        branchCode.toString()
      );
      if (getAllAccountResonse.kind === "ok") {
        setCustomerAccount(getAllAccountResonse.data.result);
        setIsLoading(false);
      }
    } else {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchAllAccount();
  }, [branchCode]);

  useEffect(() => {
    form.validateFields({ validateOnly: true });
  }, [values]);

  const handleSubmit = async () => {
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setIsLoading(true);
      const response = await depositStore.createBatchPost(values);
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
          <Button className="mb-5" type="primary" onClick={navigateToBatchHome}>
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
              name={"effectiveDate"}
              className="mb-3 flex-1"
              label="Date"
              rules={[{ required: true }]}
            >
              <DatePicker className="w-full" />
            </Form.Item>
            <Form.Item
              className="mb-3 flex-1"
              label="Branch  "
              rules={[{ required: true }]}
            >
              <Select
                placeholder="Branch "
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

          <Form.List name="transactions">
            {(fields, { add, remove }) => (
              <>
                {fields.map(({ key, name, ...restField }) => (
                  <div
                    className=" grid grid-cols-5 gap-3  justify-center  "
                    key={key}
                  >
                    <Form.Item
                      {...restField}
                      name={[name, "accountId"]}
                      rules={[
                        {
                          required: true,
                          message: "Enter account Id",
                        },
                      ]}
                      label="Account "
                      // className=" w-full"
                    >
                      <Select
                        placeholder="Select Bank"
                        className="text-gray-600"
                      >
                        {customerAccount.map(
                          ({ accountId, accountName }, index) => (
                            <Select.Option value={accountId} key={index}>
                              {accountName}
                            </Select.Option>
                          )
                        )}
                      </Select>
                    </Form.Item>
                    <Form.Item
                      {...restField}
                      name={[name, "description"]}
                      rules={[
                        {
                          required: true,
                          message: "Enter Description",
                        },
                      ]}
                      label="Description "
                    >
                      <Input placeholder="Description" />
                    </Form.Item>
                    <Form.Item
                      {...restField}
                      name={[name, "debit"]}
                      rules={[
                        {
                          required: true,
                          message: "Enter amount of debit",
                        },
                      ]}
                      label="Amount Of Debit "
                    >
                      <Input placeholder="amount of debit" type="number" />
                    </Form.Item>
                    <Form.Item
                      {...restField}
                      name={[name, "credit"]}
                      rules={[
                        {
                          required: true,
                          message: "Enter amount of credit",
                        },
                      ]}
                      label="Amount Of credit "
                    >
                      <Input
                        placeholder="Enter amount of credit"
                        type="number"
                      />
                    </Form.Item>
                    <MinusCircleOutlined onClick={() => remove(name)} />
                  </div>
                  // </Space>
                ))}

                <Form.Item>
                  <Button
                    type="dashed"
                    onClick={() => add()}
                    // block
                    icon={<PlusOutlined />}
                  >
                    Add field
                  </Button>
                </Form.Item>
              </>
            )}
          </Form.List>

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
