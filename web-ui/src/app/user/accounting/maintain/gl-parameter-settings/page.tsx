"use client";

import { useStores } from "@/hooks/use-store";
import { observer } from "mobx-react-lite";
import {
  AutoComplete,
  Button,
  Checkbox,
  DatePicker,
  Form,
  Input,
  Modal,
  Radio,
  RadioChangeEvent,
  Select,
  Space,
  Spin,
  Tooltip,
} from "antd";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { images } from "@/theme";

export default observer(function Page() {
  const { depositStore } = useStores();
  const [form] = Form.useForm();
  const values = Form.useWatch([], form);
  const [isloading, setisloading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalOpen2, setIsModalOpen2] = useState(false);
  const [allAccount, setAllAccount] = useState<[]>([]);
  const [allProduct, setallProduct] = useState<[]>([]);

  const initialValues = {
    customerProductDetail: "",
    vendorProductDetail: "",
    pettyCashAcct: "",
    payableAcct: "",
    fxAssetControlAcct: "",
    salesAcct: "",
    salesAcctIncome: "",
    purchaseAcct: "",
    purchaseAcctIncome: "",
    glOpenAcct: "",
    cotAcct: "",
    vatAcct: "",
    smsChargeAcct: "",
    smsAlertIncomeAcct: "",
    reserveAcct: "",
    custOpenAcct: "",
    cot: 0,
    vat: 0,
    smsAlert: 0,
    smsAlertCustomer: 0,
    payableChargeVAT: "",
    receivableChargeVAT: "",
    bankClearingDay: 0,
    tradingClearingDay: 0,
  };
  console.log(values);
  const router = useRouter();
  const navigateToGlParams = () => {
    router.push("/user/accounting/maintain/gl-parameter-settings");
  };

  const showModal = async () => {
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(true);
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const onSuccessDepositClose = () => {
    
    setIsModalOpen2(false);
    navigateToGlParams();
    setisloading(false);
  };
  console.log([...allProduct]);
  const fetch = async () => {
    setisloading(true);
    const fetchParamRes = await depositStore.getAllGlParams();
    const getAllAccountRes = await depositStore.getAllAccount("001");
    const getAllProductRes = await depositStore.getAllProducts();
    if (
      fetchParamRes.kind === "ok" &&
      getAllAccountRes.kind === "ok" &&
      getAllProductRes.kind === "ok"
    ) {
      const customerProductDetailFind = getAllProductRes.data.result.find(
        (value: { productCode: number }) =>
          value.productCode ===
          fetchParamRes.data.result.customerProductDetail?.productCode
      );
      const vendorDetailsFind = getAllProductRes.data.result.find(
        (value: { productCode: number }) =>
          value.productCode ===
          fetchParamRes.data.result.vendorProductDetail?.productCode
      );
      console.log(vendorDetailsFind)
      form.setFieldsValue({
        reserveAcct: fetchParamRes.data.result.reserveAccount?.accountId ?? "",
        pettyCashAcct:
          fetchParamRes.data.result.pettyCashAccount?.accountId ?? "",
        fxAssetControlAcct:
          fetchParamRes.data.result.fxAssetControlAccount?.accountId ?? "",
        vendorProductDetail:vendorDetailsFind?.productCode ?? "",
        customerProductDetail: customerProductDetailFind?.productCode ??  "",
        payableAcct: fetchParamRes.data.result.payableAccount?.accountId ?? "",
        glOpenAcct: fetchParamRes.data.result.glOpenAccount?.accountId ?? "",
        cotAcct: fetchParamRes.data.result.cotAccount?.accountId ?? "",
        vatAcct: fetchParamRes.data.result.vatAccount?.accountId ?? "",
        smsChargeAcct:
          fetchParamRes.data.result.smsChargeAccount?.accountId ?? "",
        smsAlertIncomeAcct:
          fetchParamRes.data.result.smsAlertIncomeAccount?.accountId ?? "",
        custOpenAcct:
          fetchParamRes.data.result.custOpenAccount?.accountId ?? "",
        salesAcct: fetchParamRes.data.result.salesAccount?.accountId ?? "",
        purchaseAcct:
          fetchParamRes.data.result.purchaseAccount?.accountId ?? "",
        purchaseAcctIncome:
          fetchParamRes.data.result.purchaseAccountIncome?.accountId ?? "",
        salesAcctIncome:
          fetchParamRes.data.result.salesAccountIncome?.accountId ?? "",
      });
      setAllAccount(getAllAccountRes.data.result);
      setallProduct(getAllProductRes.data.result);
      setisloading(false);
    } else {
      setisloading(false);
    }
  };

  const handleSubmit = async () => {
    console.log(values);
    if (await form.validateFields({ validateOnly: true })) {
      setIsModalOpen(false);
      setisloading(true);
      const response = await depositStore.UpdateGlParams({
        ...values,
        cot: 0,
        vat: 0,
        smsAlert: 0,
        smsAlertCustomer: 0,
        bankClearingDay: 0,
        tradingClearingDay: 0,
        payableChargeVAT: "",
        receivableChargeVAT: "",
      });
      if (response.kind === "ok") {
        setIsModalOpen(false);
        setIsModalOpen2(true);
      } else {
        setisloading(false);
      }
    }
  };
  useEffect(() => {
    fetch();
  }, []);

  return (
    <Spin tip="Authenticating...." spinning={isloading} className="text-[red] ">
      <main className="flex flex-col w-[95%] mx-auto">
        <Form
          form={form}
          initialValues={initialValues}
          className="bg-gray-50 p-8 rounded border"
          layout="vertical"
        >
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"reserveAcct"}
              className="mb-3 flex-1"
              label="Reserve Account "
            >
              <Select placeholder="reserve Account " className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"pettyCashAcct"}
              className="mb-3 flex-1"
              label="petty Cash Account "
            >
              <Select placeholder="pettyCashAcct" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"fxAssetControlAcct"}
              className="mb-3 flex-1"
              label="fx-Asset Control Account"
            >
              <Select
                placeholder="fxAssetControlAccount"
                className="text-gray-600"
              >
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"vendorProductDetail"}
              className="mb-3 flex-1"
              label="vendor Product Account "
            >
              <Select
                placeholder="vendor Product Account"
                className="text-gray-600"
              >
                {allProduct.map(({ productCode, productName }, index) => (
                  <Select.Option value={productCode} key={index}>
                    {productName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"customerProductDetail"}
              className="mb-3 flex-1"
              label="customer Product Account"
            >
              <Select
                placeholder="customer Product Account"
                className="text-gray-600"
              >
                {allProduct.map(({ productCode, productName }, index) => (
                  <Select.Option value={productCode} key={index}>
                    {productName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"payableAcct"}
              className="mb-3 flex-1"
              label="payable Account"
            >
              <Select placeholder="payable Account" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"glOpenAcct"}
              className="mb-3 flex-1"
              label="glOpen Account "
            >
              <Select placeholder="glOpen Account " className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"cotAcct"}
              className="mb-3 flex-1"
              label="Cot Account "
            >
              <Select placeholder="cot Account" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"vatAcct"}
              className="mb-3 flex-1"
              label="vat Account"
            >
              <Select placeholder="vatAcct" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"smsChargeAcct"}
              className="mb-3 flex-1"
              label="sms Charge Account "
            >
              <Select
                placeholder="sms Charge Account "
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
              name={"smsAlertIncomeAcct"}
              className="mb-3 flex-1"
              label="sms Alert Income Account"
            >
              <Select
                placeholder="sms Alert Income Account"
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
              name={"custOpenAcct"}
              className="mb-3 flex-1"
              label="cust open Account"
            >
              <Select placeholder="cust open Account" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"salesAcct"}
              className="mb-3 flex-1"
              label="Sales  Account "
            >
              <Select placeholder="Sales Account " className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>

            <Form.Item
              name={"salesAcctIncome"}
              className="mb-3 flex-1"
              label="Sales Account Income"
            >
              <Select
                placeholder="Sales Account Income"
                className="text-gray-600"
              >
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </div>
          <div className="grid grid-cols-3 gap-3 ">
            <Form.Item
              name={"purchaseAcct"}
              className="mb-3 flex-1"
              label="Purchase Account "
            >
              <Select placeholder="Purchase Account" className="text-gray-600">
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
            <Form.Item
              name={"purchaseAcctIncome"}
              className="mb-3 flex-1"
              label="Purchase Account Income"
            >
              <Select
                placeholder="Purchase Account Income"
                className="text-gray-600"
              >
                {allAccount.map(({ accountId, accountName }, index) => (
                  <Select.Option value={accountId} key={index}>
                    {accountName}
                  </Select.Option>
                ))}
              </Select>
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
