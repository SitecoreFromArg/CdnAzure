AZURE CDN CONNECTOR
===================

A Content Delivery Network (CDN) works by providing alternative server nodes for users to download resources (usually static content like images and JavaScript). These nodes spread throughout the world, therefore being geographically closer to your users, ensuring a faster response and download time of content due to reduced latency.

Microsoft provide a CDN service on the cloud. This service needs to take the static resources from a local folder on the same website. This package adds to Sitecore the feature to clone all the files of the Media Library into this folder. This action is going to happen just when you publish those items from the Master database to Web database.

After this, some extra configurations are necessary on the Content Delivery servers to take them from the CDN servers.

More details on this site: https://marketplace.sitecore.net/Modules/Azure_CDN_Connector.aspx


INSTALLATION STEPS:
-------------------

After the installation of this package perform the following actions:

- Edit the configuration file of this package with the Azure CDN data (/App_Config/Include/SitecoreFromArg.CdnAzure.config)

- Edit the Web.config of the CD servers to change the following parameters:
 * Media.AlwaysIncludeServerUrl = true
 * Media.MediaLinkPrefix = "~/"
 * Media.MediaLinkServerUrl = Azure CDN Url (e.g.:"http://aabbccdd.vo.msecnd.net")
 * Media.RequestExtension (remove attribute value="ashx")
 
CONFIGURATIONS:
---------------

The configuration file has the following parameters:

Enabled: This parameter allow you to disable this component.
<Enabled>yes</Enabled>

SourceDomain: On this parameter you can change the default folder name that Azure assign by default.
<SourceDomain>/cdn</SourceDomain>


